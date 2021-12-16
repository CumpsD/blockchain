namespace Blockchain.Peers
{
    using System.Text;
    using Infrastructure;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private void HandleNewBlock(
            Message<NewBlockMessage> newBlockMessage)
        {
            var graffiti = newBlockMessage.Payload.Block.Header.Graffiti.ConvertFromHex(Encoding.UTF8);

            _logger.LogTrace(
                "[{Address,15}] New block from {Graffiti} ({Identity} / {Name}) - {Sequence}",
                Address,
                graffiti,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                newBlockMessage.Payload.Block.Header.Sequence);

            _newBlockLogger.LogDebug(
                "[{Address,15}] New block from {Graffiti} ({Identity} / {Name}) - {Sequence}",
                Address,
                graffiti,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                newBlockMessage.Payload.Block.Header.Sequence);
        }
    }
}
