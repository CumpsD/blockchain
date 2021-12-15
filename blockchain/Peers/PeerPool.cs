namespace Blockchain.Peers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    [UsedImplicitly]
    public class PeerPool
    {
        private readonly ILogger<PeerPool> _logger;
        private readonly PeerFactory _peerFactory;

        private Dictionary<string, Peer> Peers { get; } = new();

        public PeerPool(
            ILogger<PeerPool> logger,
            PeerFactory peerFactory)
        {
            _logger = logger;
            _peerFactory = peerFactory;
        }

        public void AddPeer(
            string? address,
            int port,
            string? identity = null,
            string? name = null,
            CancellationToken ct = default)
        {
            if (address == null)
                return;

            if (Peers.ContainsKey(address))
                return;

            _logger.LogDebug(
                "[{Address}] Adding peer {Address}:{Port} ({Identity} / {Name})",
                address,
                address,
                port,
                identity,
                string.IsNullOrWhiteSpace(name) ? "*" : name);

            var peer = _peerFactory.GetPeer(
                this,
                address,
                port,
                identity,
                name);

            Peers.TryAdd(peer.Address, peer);

            peer.ConnectAndListen(ct);
        }

        public List<Peer> GetPeers()
            => Peers.Values.ToList();

        public void UpdatePeers(CancellationToken ct)
        {
            foreach (var peer in Peers.Values)
                peer.PeerListRequestAsync(ct);
        }

        public void RemovePeer(string? address)
        {
            if (address == null)
                return;

            if (!Peers.ContainsKey(address))
                return;

            var peer = Peers[address];

            _logger.LogDebug(
                "[{Address}] Removing peer {Address}:{Port} ({Identity} / {Name})",
                peer.Address,
                peer.Address,
                peer.Port,
                peer.Identity,
                string.IsNullOrWhiteSpace(peer.Name) ? "*" : peer.Name);

            Peers.Remove(address);

            peer.Disconnect();
        }
    }
}
