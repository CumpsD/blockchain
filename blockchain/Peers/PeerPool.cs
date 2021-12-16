namespace Blockchain.Peers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Configuration;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [UsedImplicitly]
    public class PeerPool
    {
        private readonly ILogger<PeerPool> _logger;
        private readonly PeerFactory _peerFactory;
        private readonly BlockchainConfiguration _configuration;

        private ConcurrentDictionary<string, Peer> Peers { get; } = new();

        public PeerPool(
            ILogger<PeerPool> logger,
            IOptions<BlockchainConfiguration> options,
            PeerFactory peerFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = options.Value ?? throw new ArgumentNullException(nameof(options));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
        }

        public int GetConnectedPeerCount()
            => Peers
                .Values
                .Count(peer => peer.IsConnected);

        public IEnumerable<Peer> GetConnectedPeers()
            => Peers
                .Values
                .Where(peer => peer.IsConnected)
                .AsEnumerable();

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

            var peerCount = GetConnectedPeerCount();
            if (peerCount >= _configuration.TargetPeerCount)
            {
                _logger.LogTrace(
                    "[{Address,15}] Not adding peer {Address}:{Port} ({Identity} / {Name}), we have enough ({PeerCount})",
                    address,
                    address,
                    port,
                    identity,
                    string.IsNullOrWhiteSpace(name) ? "*" : name,
                    peerCount);

                return;
            }

            _logger.LogDebug(
                "[{Address,15}] Adding peer {Address}:{Port} ({Identity} / {Name})",
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

        public void UpdatePeerLists(CancellationToken ct)
        {
            foreach (var peer in GetConnectedPeers())
                peer.PeerListRequestAsync(ct);
        }

        public void RemovePeer(
            string? address,
            string reason)
        {
            if (address == null)
                return;

            if (!Peers.ContainsKey(address))
                return;

            var peer = Peers[address];

            _logger.LogDebug(
                "[{Address,15}] Removing peer {Address}:{Port} ({Identity} / {Name}) - {Reason}",
                peer.Address,
                peer.Address,
                peer.Port,
                peer.Identity,
                string.IsNullOrWhiteSpace(peer.Name) ? "*" : peer.Name,
                reason);

            Peers.Remove(address, out _);

            peer.Disconnect();
        }
    }
}
