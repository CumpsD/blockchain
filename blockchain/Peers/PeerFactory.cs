namespace Blockchain.Peers
{
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly]
    public class PeerFactory
    {
        private readonly ILogger<Peer> _logger;

        public PeerFactory(ILogger<Peer> logger)
            => _logger = logger;

        public Peer GetPeer(
            PeerPool peerPool,
            string address,
            int port)
            => new(
                _logger,
                peerPool,
                address,
                port);
    }
}
