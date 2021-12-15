namespace Blockchain.Peers
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
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

        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private readonly ILogger<Peer> _logger;
        private readonly BlockchainConfiguration _configuration;
        private readonly PeerPool _peerPool;

        private ClientWebSocket? _ws;

        public string? Identity { get; private set; }

        public string? Name { get; private set; }

        public string Address { get; }

        public int Port { get; private set; }

        public Peer(
            ILogger<Peer> logger,
            BlockchainConfiguration configuration,
            PeerPool peerPool,
            string address,
            int port,
            string? identity,
            string? name)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            while (ct.IsCancellationRequested == false)
            {
                try
                {
                    _ws = new ClientWebSocket();

                    await ConnectAsync($"{Address}:{Port}", ct);
                    await IdentityAsync(ct);

                    while (_ws.State == WebSocketState.Open && ct.IsCancellationRequested == false)
                    {
                        var buffer = new byte[50000];
                        var result = await _ws.ReceiveAsync(buffer, ct);

                        if (result.EndOfMessage)
                            await Dispatch(
                                result,
                                buffer,
                                ct);
                    }
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
                    _ws?.Dispose();
                    _ws = null;
                }
            }
        }

        private async Task Dispatch(
            WebSocketReceiveResult result,
            byte[] buffer,
            CancellationToken ct)
        {
            if (result.MessageType != WebSocketMessageType.Text)
                return;

            Message? message = null;

            try
            {
                message = JsonSerializer.Deserialize<Message>(
                    new ReadOnlySpan<byte>(buffer, 0, result.Count),
                    _deserializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Address}] Invalid incoming message: {@Message}",
                    Address,
                    Encoding.UTF8.GetString(buffer));
            }

            if (message == null)
                return;

            _logger.LogDebug(
                "[{Address}] Incoming Message: {@Message}",
                Address,
                message);

            switch (message.Type)
            {
                case InternalMessageType.Identity:
                    var identityMessage = (Message<IdentityMessage>)message;
                    HandleIdentity(identityMessage);
                    break;

                case InternalMessageType.PeerListRequest:
                    await HandlePeerListRequest(ct);
                    break;

                case InternalMessageType.PeerList:
                    var peerListMessage = (Message<PeerListMessage>)message;
                    HandlePeerList(peerListMessage, ct);
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

            _logger.LogDebug(
                "[{Address}] Outgoing Message: {@Message}",
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
    }
}
