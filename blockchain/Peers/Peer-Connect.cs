namespace Blockchain.Peers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private async Task<bool> ConnectAsync(
            string peer,
            CancellationToken ct)
        {
            if (_ws == null)
                return false;

            _logger.LogDebug(
                "[{Address}] Connecting to {Peer}",
                Address,
                peer);

            try
            {
                await _ws.ConnectAsync(new Uri($"ws://{peer}"), ct);

                _logger.LogDebug(
                    "[{Address}] Connection is {ConnectionState}",
                    Address,
                    _ws.State);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{Address}] Connection is {ConnectionState}",
                    Address,
                    _ws.State);

                _peerPool.RemovePeer(Address);

                return false;
            }
        }
    }
}
