namespace Blockchain
{
    using System;
    using System.Net.WebSockets;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Messages;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Runner
    {
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
            await ws.ConnectAsync(new Uri("ws://127.0.0.1:8080"), ct);

            await SendAsync(
                new Identify(
                    identity: "bwBpkJX5VAtFOt2K/DFuJ+KrlkWy2YNDkxe19TKnlCI=",
                    version: 11,
                    agent: "if/cli/4b3079f",
                    name: "csharp-node",
                    port: 9033,
                    head: "8fb5b576eb519e03911c8768d7c8ac97252f7be49fec77279c43af2da9f82068",
                    work: "131072",
                    sequence: 1),
                ws,
                ct);

            Console.WriteLine(ws.State);
        }

        private static async Task SendAsync<T>(
            IMessage<T> message,
            WebSocket ws,
            CancellationToken ct)
        {
            var x = message.CreateMessage();
            var z = JsonSerializer.Serialize(x);

            await ws.SendAsync(
                new ArraySegment<byte>(
                    JsonSerializer.SerializeToUtf8Bytes(message.CreateMessage())),
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                ct);
        }
    }
}
