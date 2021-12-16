namespace Blockchain.Peers
{
    using System;
    using Configuration;
    using JetBrains.Annotations;
    using Loggers;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [UsedImplicitly]
    public class PeerFactory
    {
        private readonly ILogger<Peer> _logger;

        private readonly ILogger<ConnectedLogger> _connectedLogger;
        private readonly ILogger<DisconnectedLogger> _disconnectedLogger;
        private readonly ILogger<NewBlockLogger> _newBlockLogger;

        private readonly BlockchainConfiguration _configuration;

        public PeerFactory(
            ILogger<Peer> logger,
            ILoggerFactory loggerFactory,
            IOptions<BlockchainConfiguration> options)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));

            _connectedLogger = loggerFactory.CreateLogger<ConnectedLogger>();
            _disconnectedLogger = loggerFactory.CreateLogger<DisconnectedLogger>();
            _newBlockLogger = loggerFactory.CreateLogger<NewBlockLogger>();
        }

        public Peer GetPeer(
            PeerPool peerPool,
            string address,
            int port,
            string? identity,
            string? name)
            => new(
                _logger,
                _connectedLogger,
                _disconnectedLogger,
                _newBlockLogger,
                _configuration,
                peerPool,
                address,
                port,
                identity,
                name);
    }
}
