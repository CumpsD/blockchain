namespace Blockchain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
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
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting {TaskName}.", nameof(Runner));

            await Task.Delay(10000, cancellationToken);

            // TODO: Implement
        }
    }
}
