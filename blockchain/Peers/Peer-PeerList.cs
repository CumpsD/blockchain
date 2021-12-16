namespace Blockchain.Peers
{
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private async Task PeerListAsync(
            CancellationToken ct)
        {
            if (_ws is not { State: WebSocketState.Open })
                return;

            var peers = _peerPool
                .GetPeers()
                .Where(peer => peer.Identity != null)
                .Select(peer =>
                    new ConnectedPeer(
                        peer.Identity!,
                        peer.Name,
                        peer.Address,
                        peer.Port))
                .ToArray();

            _logger.LogTrace(
                "[{Address,15}] Sending peer list ({TotalPeers} peers)",
                Address,
                peers.Length);

            await SendAsync(
                new PeerListMessage(peers),
                ct);
        }

        private void HandlePeerList(
            Message<PeerListMessage> peerListMessage,
            CancellationToken ct)
        {
            _logger.LogTrace(
                "[{Address,15}] Received peer list ({TotalPeers} peers)",
                Address,
                peerListMessage.Payload.ConnectedPeers.Length);

            foreach (var peer in peerListMessage.Payload.ConnectedPeers)
                _peerPool.AddPeer(
                    peer.Address,
                    peer.Port ?? _configuration.DefaultWebsocketPort.Value,
                    peer.Identity,
                    peer.Name,
                    ct);
        }
    }
}
