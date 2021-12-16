namespace Blockchain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Peers;
    using Timer = System.Timers.Timer;

    public class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly PeerPool _peerPool;

        private readonly BlockchainConfiguration _configuration;

        private readonly Timer _updatePeerListTimer;
        private readonly Timer _printPeerInfoTimer;

        public Runner(
            ILogger<Runner> logger,
            IOptions<BlockchainConfiguration> options,
            PeerPool peerPool)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _peerPool = peerPool ?? throw new ArgumentNullException(nameof(peerPool));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));

            _updatePeerListTimer = new(_configuration.UpdatePeerListIntervalInSeconds.Value * 1000) { AutoReset = true };
            _printPeerInfoTimer = new(_configuration.PrintPeerInfoIntervalInSeconds.Value * 1000) { AutoReset = true };
        }

        public async Task StartAsync(
            CancellationToken ct)
        {
            _logger.LogInformation("Starting {TaskName}.", nameof(Runner));

            foreach (var bootstrap in _configuration.BootstrapNodeAddresses)
            {
                var (address, port) = ParseAddress(bootstrap);

                _peerPool.AddPeer(
                    address,
                    port,
                    ct: ct);
            }

            _updatePeerListTimer.Elapsed += (_, _) => UpdatePeerList(ct);
            _updatePeerListTimer.Start();

            _printPeerInfoTimer.Elapsed += (_, _) => PrintPeerInfo();
            _printPeerInfoTimer.Start();

            await Task.Delay(-1, ct);
        }

        private (string, int) ParseAddress(string address)
        {
            var addressParts = address.Split(':', StringSplitOptions.TrimEntries);

            return (addressParts.Length == 1)
                ? (addressParts[0], _configuration.DefaultWebsocketPort.Value)
                : (addressParts[0], int.Parse(addressParts[1]));
        }

        private void UpdatePeerList(CancellationToken ct)
            => _peerPool.UpdatePeerLists(ct);

        private void PrintPeerInfo()
            => _logger.LogInformation(
                "Connected to {NumberOfPeers} peers.",
                _peerPool.GetConnectedPeerCount());
    }
}
