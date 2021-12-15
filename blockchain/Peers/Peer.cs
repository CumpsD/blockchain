namespace Blockchain.Peers
{
    using System;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;
    using Microsoft.Extensions.Logging;

    public class Peer
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
        private readonly PeerPool _peerPool;

        public string? Identity { get; private set; }

        public string? Name { get; private set; }

        public string Address { get; }

        public int Port { get; private set; }

        public Peer(
            ILogger<Peer> logger,
            PeerPool peerPool,
            string address,
            int port)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _peerPool = peerPool ?? throw new ArgumentNullException(nameof(peerPool));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
        }

        public async Task ConnectAndListen(
            CancellationToken ct)
        {
            while (ct.IsCancellationRequested == false)
            {
                var ws = new ClientWebSocket();

                try
                {
                    await ConnectAsync($"{Address}:{Port}", ws, ct);
                    await IdentifyAsync(ws, ct);

                    while (ws.State == WebSocketState.Open && ct.IsCancellationRequested == false)
                    {
                        var buffer = new byte[4000];
                        var result = await ws.ReceiveAsync(buffer, ct);

                        if (result.EndOfMessage)
                            await Dispatch(
                                result,
                                buffer,
                                ws,
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
                    ws.Dispose();
                }
            }
        }

        private async Task ConnectAsync(
            string peer,
            ClientWebSocket ws,
            CancellationToken ct)
        {
            _logger.LogDebug("Connecting to {Peer}", peer);

            await ws.ConnectAsync(new Uri($"ws://{peer}"), ct);

            _logger.LogDebug("Connection is {ConnectionState}", ws.State);
        }

        private async Task IdentifyAsync(
            WebSocket ws,
            CancellationToken ct)
        {
            if (ws.State != WebSocketState.Open)
                return;

            _logger.LogDebug("Sending identity");

            // TODO: Get sensible values
            await SendAsync(
                new IdentityMessage(
                    identity: "GhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGhoaGho=",
                    version: 11,
                    agent: "csharp",
                    name: "csharp-node",
                    port: 9033,
                    head: "0000000017996e3b061fb7118db7007084000a28306a285f193b2551854343bd",
                    work: "1414804792318651",
                    sequence: 23123),
                ws,
                ct);
        }

        private async Task Dispatch(
            WebSocketReceiveResult result,
            byte[] buffer,
            WebSocket ws,
            CancellationToken ct)
        {
            var message = JsonSerializer.Deserialize<Message>(
                new ReadOnlySpan<byte>(buffer, 0, result.Count),
                _deserializerOptions);

            _logger.LogDebug("Incoming Message: {@Message}", message);

            switch (message.Type)
            {
                case InternalMessageType.Identity:
                    var identityMessage = (Message<IdentityMessage>)message;
                    HandleIdentity(identityMessage);
                    break;

                case InternalMessageType.PeerListRequest:
                    await PeerListAsync(ws, ct);
                    break;

                case InternalMessageType.PeerList:
                    var peerListMessage = (Message<PeerListMessage>)message;

                    break;
            }
        }

        private void HandleIdentity(Message<IdentityMessage> identityMessage)
        {
            var payLoad = identityMessage.Payload;

            _logger.LogDebug(
                "Updating identity {Identity}",
                payLoad.Identity);

            Identity = payLoad.Identity;

            if (payLoad.Port.HasValue)
                Port = payLoad.Port.Value;

            if (payLoad.Name != null)
                Name = payLoad.Name;
        }

        private async Task PeerListAsync(
            WebSocket ws,
            CancellationToken ct)
        {
            if (ws.State != WebSocketState.Open)
                return;

            var peers = _peerPool
                .GetPeers()
                .Where(peer => peer.Identity != null)
                .Select(peer =>
                    new ConnectedPeer(
                        peer.Identity,
                        peer.Name,
                        peer.Address,
                        peer.Port))
                .ToArray();

            _logger.LogDebug("Sending peer list (#{TotalPeers})", peers.Length);

            await SendAsync(
                new PeerListMessage(peers),
                ws,
                ct);
        }

        private async Task SendAsync<T>(
            IMessage<T> message,
            WebSocket ws,
            CancellationToken ct)
        {
            var outgoingMessage = message.CreateMessage();

            _logger.LogDebug("Outgoing Message: {@Message}", outgoingMessage);

            // Locking because you can only have 1 send at a time for a WS
            await _sendLock.WaitAsync(ct);
            try
            {
                await ws.SendAsync(
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
