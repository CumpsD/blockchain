namespace Blockchain.Peers
{
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private void HandleDisconnecting(
            Message<DisconnectingMessage> disconnectingMessage)
        {
            _logger.LogTrace(
                "[{Address,15}] Disconnecting from {Peer} - {Reason}",
                Address,
                disconnectingMessage.Payload.SourceIdentity,
                disconnectingMessage.Payload.Reason);

            _peerPool.RemovePeer(
                Address,
                $"Disconnect requested, {disconnectingMessage.Payload.Reason}");
        }
    }
}
