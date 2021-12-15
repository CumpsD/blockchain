namespace Blockchain
{
    using System;
    using System.Net.WebSockets;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Messages;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Runner
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

        private readonly ILogger<Runner> _logger;

        private readonly BlockchainConfiguration _configuration;

        public Runner(
            ILogger<Runner> logger,
            IOptions<BlockchainConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task StartAsync(
            CancellationToken ct)
        {
            _logger.LogInformation("Starting {TaskName}.", nameof(Runner));

            await ConnectAndListen(ct);
        }

        private async Task ConnectAndListen(
            CancellationToken ct)
        {
            var peer = "127.0.0.1:8080";

            while (ct.IsCancellationRequested == false)
            {
                var ws = new ClientWebSocket();

                try
                {
                    await ConnectAsync(peer, ws, ct);
                    await IdentifyAsync(ws, ct);

                    while (ws.State == WebSocketState.Open && ct.IsCancellationRequested == false)
                    {
                        var buffer = new byte[4000];
                        var result = await ws.ReceiveAsync(buffer, ct);

                        if (result.EndOfMessage)
                            await Dispatch(result, buffer);
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

        private async Task Dispatch(WebSocketReceiveResult result, byte[] buffer)
        {
            var message = JsonSerializer.Deserialize<Message>(
                new ReadOnlySpan<byte>(buffer, 0, result.Count),
                _deserializerOptions);

            _logger.LogDebug("Incoming Message: {@Message}", message);
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
                new Identify(
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

        private static async Task SendAsync<T>(
            IMessage<T> message,
            WebSocket ws,
            CancellationToken ct)
        {
            await ws.SendAsync(
                new ArraySegment<byte>(
                    JsonSerializer.SerializeToUtf8Bytes(message.CreateMessage(), _serializerOptions)),
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                ct);
        }
    }
}
