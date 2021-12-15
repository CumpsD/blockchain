namespace Blockchain.Peers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private async Task ConnectAsync(
            string peer,
            CancellationToken ct)
        {
            if (_ws == null)
                return;

            _logger.LogDebug(
                "[{Address}] Connecting to {Peer}",
                Address,
                peer);

            await _ws.ConnectAsync(new Uri($"ws://{peer}"), ct);

            _logger.LogDebug(
                "[{Address}] Connection is {ConnectionState}",
                Address,
                _ws.State);
        }
    }
}
