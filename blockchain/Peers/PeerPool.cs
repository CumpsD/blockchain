namespace Blockchain.Peers
{
    using System.Collections.Generic;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly]
    public class PeerPool
    {
        private readonly ILogger<PeerPool> _logger;
        private readonly PeerFactory _peerFactory;

        private Dictionary<string, Peer> Peers { get; }= new();

        public PeerPool(
            ILogger<PeerPool> logger,
            PeerFactory peerFactory)
        {
            _logger = logger;
            _peerFactory = peerFactory;
        }

        public Peer GetPeer(
            string address,
            int port,
            CancellationToken ct)
        {
            var peer = _peerFactory.GetPeer(
                this,
                address,
                port);

            Peers.TryAdd(peer.Address, peer);

            peer.ConnectAndListen(ct);

            return peer;
        }
    }
}
