namespace Blockchain.Peers
{
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private void HandleNewBlock(
            Message<NewBlockMessage> newBlockMessage)
        {
            _logger.LogTrace(
                "[{Address,15}] New block from {Identity} / {Name}) - {Sequence}",
                Address,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                newBlockMessage.Payload.Block.Header.Sequence);

            _newBlockLogger.LogDebug(
                "[{Address,15}] New block from {Identity} / {Name}) - {Sequence}",
                Address,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                newBlockMessage.Payload.Block.Header.Sequence);
        }
    }
}
