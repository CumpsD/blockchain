namespace Blockchain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Peers;

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
            IServiceProvider container,
            CancellationToken ct)
        {
            _logger.LogInformation("Starting {TaskName}.", nameof(Runner));

            var bootstrapPeer = container
                .GetRequiredService<PeerFactory>()
                .GetPeer("127.0.0.1:8080");

            await bootstrapPeer.ConnectAndListen(ct);
        }
    }
}
