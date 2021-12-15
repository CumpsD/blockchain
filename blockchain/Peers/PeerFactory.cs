namespace Blockchain.Peers
{
    using System;
    using Configuration;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [UsedImplicitly]
    public class PeerFactory
    {
        private readonly ILogger<Peer> _logger;
        private readonly BlockchainConfiguration _configuration;

        public PeerFactory(
            ILogger<Peer> logger,
            IOptions<BlockchainConfiguration> options)
        {
            _logger = logger;
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Peer GetPeer(
            PeerPool peerPool,
            string address,
            int port,
            string? identity,
            string? name)
            => new(
                _logger,
                _configuration,
                peerPool,
                address,
                port,
                identity,
                name);
    }
}
