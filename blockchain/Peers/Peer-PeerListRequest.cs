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
        public async Task PeerListRequestAsync(
            CancellationToken ct)
        {
            if (_ws is not { State: WebSocketState.Open })
                return;

            _logger.LogDebug(
                "[{Address}] Requesting peer list from {Identity} / {Name}",
                Address,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name);

            await SendAsync(
                new PeerListRequestMessage(),
                ct);
        }

        private async Task HandlePeerListRequest(CancellationToken ct)
            => await PeerListAsync(ct);
    }
}
