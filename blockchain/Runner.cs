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
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
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

            await Bla(ct);

            // TODO: Implement
        }

        private async Task Bla(
            CancellationToken ct)
        {
            var ws = new ClientWebSocket();
            var peer = "127.0.0.1:8080";

            _logger.LogDebug("Connecting to {Peer}", peer);
            await ws.ConnectAsync(new Uri($"ws://{peer}"), ct);
            await Task.Delay(500, ct);

            _logger.LogDebug("Sending identity");
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

            Console.WriteLine(ws.State);

            Console.ReadLine();
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
