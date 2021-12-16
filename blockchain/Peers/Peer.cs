namespace Blockchain.Peers
{
    using System;
    using System.Buffers;
    using System.Net.WebSockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Loggers;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private const int RECEIVE_CHUNK_SIZE = 1000;

        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            }
        };

        private static readonly JsonSerializerOptions _deserializerOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new MessageConverter()
            }
        };

        private readonly ILogger<Peer> _logger;
        private readonly ILogger<ConnectedLogger> _connectedLogger;
        private readonly ILogger<DisconnectedLogger> _disconnectedLogger;
        private readonly ILogger<NewBlockLogger> _newBlockLogger;

        private readonly SemaphoreSlim _sendLock = new(1, 1);

        private readonly BlockchainConfiguration _configuration;

        private readonly PeerPool _peerPool;

        private ClientWebSocket? _ws;

        private bool _shouldDisconnect;

        public string? Identity { get; private set; }

        public string? Name { get; private set; }

        public string Address { get; }

        public int Port { get; private set; }

        public bool IsConnected
        {
            get => _ws is { State: WebSocketState.Open };
        }

        public Peer(
            ILogger<Peer> logger,
            ILogger<ConnectedLogger> connectedLogger,
            ILogger<DisconnectedLogger> disconnectedLogger,
            ILogger<NewBlockLogger> newBlockLogger,
            BlockchainConfiguration configuration,
            PeerPool peerPool,
            string address,
            int port,
            string? identity,
            string? name)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectedLogger = connectedLogger ?? throw new ArgumentNullException(nameof(connectedLogger));
            _disconnectedLogger = disconnectedLogger ?? throw new ArgumentNullException(nameof(disconnectedLogger));
            _newBlockLogger = newBlockLogger ?? throw new ArgumentNullException(nameof(newBlockLogger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _peerPool = peerPool ?? throw new ArgumentNullException(nameof(peerPool));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
            Identity = identity;
            Name = name;
        }

        public async Task ConnectAndListen(
            CancellationToken ct)
        {
            var resultProcessor = new WebSocketReceiveResultProcessor();

            while (ct.IsCancellationRequested == false && !_shouldDisconnect)
            {
                try
                {
                    _ws = new ClientWebSocket();

                    var connected = await ConnectAsync($"{Address}:{Port}", ct);
                    if (!connected)
                    {
                        try
                        {
                            await Task.Delay(5000, ct);
                        }
                        catch
                        {
                            // ignored
                        }

                        continue;
                    }

                    await IdentityAsync(ct);

                    while (_ws?.State == WebSocketState.Open && ct.IsCancellationRequested == false)
                    {
                        var buffer = ArrayPool<byte>.Shared.Rent(RECEIVE_CHUNK_SIZE);

                        var result = await _ws.ReceiveAsync(buffer, ct);
                        var isEndOfMessage = resultProcessor.Receive(result, buffer, out var frame);

                        if (isEndOfMessage)
                        {
                            if (frame.IsEmpty)
                                break; // End of message with no data means socket closed - break so we can reconnect.

                            await Dispatch(
                                result,
                                frame,
                                ct);
                        }
                    }

                    _logger.LogTrace(
                        "[{Address,15}] Connection is {ConnectionState}",
                        Address,
                        _ws?.State);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, ex.Message);
                    return;
                }
                finally
                {
                    resultProcessor.Dispose();
                    _ws?.Dispose();
                    _ws = null;
                }
            }
        }

        private async Task Dispatch(
            WebSocketReceiveResult result,
            ReadOnlySequence<byte> frame,
            CancellationToken ct)
        {
            if (result.MessageType != WebSocketMessageType.Text)
                return;

            Message? message = null;

            try
            {
                message = JsonSerializer.Deserialize<Message>(
                    frame.ToArray(),
                    _deserializerOptions);

                foreach (var chunk in frame)
                {
                    if (MemoryMarshal.TryGetArray(chunk, out var segment) && segment.Array != null)
                        ArrayPool<byte>.Shared.Return(segment.Array);
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(
                    ex,
                    "[{Address,15}] Invalid incoming message: {@Message}",
                    Address,
                    Encoding.UTF8.GetString(frame.ToArray()));
            }

            if (message == null)
                return;

            _logger.LogTrace(
                "[{Address,15}] Incoming Message: {@Message}",
                Address,
                message);

            switch (message.Type)
            {
                case MessageType.Identity:
                    var identityMessage = (Message<IdentityMessage>)message;
                    HandleIdentity(identityMessage);
                    break;

                case MessageType.PeerListRequest:
                    await HandlePeerListRequest(ct);
                    break;

                case MessageType.PeerList:
                    var peerListMessage = (Message<PeerListMessage>)message;
                    HandlePeerList(peerListMessage, ct);
                    break;

                case MessageType.Disconnecting:
                    var disconnectingMessage = (Message<DisconnectingMessage>)message;
                    HandleDisconnecting(disconnectingMessage);
                    break;

                case MessageType.NewBlock:
                    var newBlockMessage = (Message<NewBlockMessage>)message;
                    HandleNewBlock(newBlockMessage);
                    break;
            }
        }

        private async Task SendAsync<T>(
            IMessage<T> message,
            CancellationToken ct)
        {
            if (_ws is not { State: WebSocketState.Open })
                return;

            var outgoingMessage = message.CreateMessage();

            _logger.LogTrace(
                "[{Address,15}] Outgoing Message: {@Message}",
                Address,
                outgoingMessage);

            // Locking because you can only have 1 send at a time for a WS
            await _sendLock.WaitAsync(ct);
            try
            {
                await _ws.SendAsync(
                    new ArraySegment<byte>(
                        JsonSerializer.SerializeToUtf8Bytes(outgoingMessage, _serializerOptions)),
                    WebSocketMessageType.Text,
                    WebSocketMessageFlags.EndOfMessage,
                    ct);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Disconnect()
        {
            _shouldDisconnect = true;

            _ws?.Dispose();
            _ws = null;
        }
    }

    public class Chunk<T> : ReadOnlySequenceSegment<T>
    {
        public Chunk(ReadOnlyMemory<T> memory)
            => Memory = memory;

        public Chunk<T> Add(ReadOnlyMemory<T> memory)
        {
            var segment = new Chunk<T>(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };

            Next = segment;
            return segment;
        }
    }

    public sealed class WebSocketReceiveResultProcessor : IDisposable
    {
        Chunk<byte>? _startChunk;
        Chunk<byte>? _currentChunk;

        public bool Receive(
            WebSocketReceiveResult result,
            ArraySegment<byte> buffer,
            out ReadOnlySequence<byte> frame)
        {
            if (result.EndOfMessage && result.MessageType == WebSocketMessageType.Close)
            {
                frame = default;
                return false;
            }

            // If not using array pool, take a local copy to avoid corruption as buffer is reused by caller.
            var slice = buffer[..result.Count];

            if (_startChunk == null)
                _startChunk = _currentChunk = new Chunk<byte>(slice);
            else
                _currentChunk = _currentChunk.Add(slice);

            if (result.EndOfMessage && _startChunk != null)
            {

                frame = _startChunk.Next == null
                    ? new ReadOnlySequence<byte>(_startChunk.Memory)
                    : new ReadOnlySequence<byte>(_startChunk, 0, _currentChunk, _currentChunk.Memory.Length);

                // Reset so we can accept new chunks from scratch.
                _startChunk = _currentChunk = null;
                return true;
            }
            else
            {
                frame = default;
                return false;
            }
        }

        public void Dispose()
        {
            var chunk = _startChunk;

            while (chunk != null)
            {
                if (MemoryMarshal.TryGetArray(chunk.Memory, out var segment) && segment.Array != null)
                    ArrayPool<byte>.Shared.Return(segment.Array);

                chunk = (Chunk<byte>)chunk.Next;
            }

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
