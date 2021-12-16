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

            _logger.LogTrace(
                "[{Address,15}] Connecting to {Peer}",
                Address,
                peer);

            try
            {
                await _ws.ConnectAsync(new Uri($"ws://{peer}"), ct);

                _logger.LogTrace(
                    "[{Address,15}] Connection is {ConnectionState}",
                    Address,
                    _ws?.State);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "[{Address,15}] Connection is {ConnectionState}",
                    Address,
                    _ws?.State);

                _peerPool.RemovePeer(Address);

                return false;
            }
        }
    }
}
