namespace Blockchain.Peers
{
    using System.Numerics;
    using System.Text;
    using Infrastructure;
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private void HandleNewBlock(
            Message<NewBlockMessage> newBlockMessage)
        {
            var block = newBlockMessage.Payload.Block;
            var graffiti = block.Header.Graffiti.ConvertFromHex(Encoding.UTF8);

            _logger.LogTrace(
                "[{Address,15}] New block from {Graffiti} ({Identity} / {Name}) - {Sequence} - {Work}",
                Address,
                graffiti,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                block.Header.Sequence,
                block.Header.Work);

            _newBlockLogger.LogDebug(
                "[{Address,15}] New block from {Graffiti} ({Identity} / {Name}) - {Sequence} - {Work}",
                Address,
                graffiti,
                Identity,
                string.IsNullOrWhiteSpace(Name) ? "*" : Name,
                block.Header.Sequence,
                block.Header.Work);

            // TODO: Add loads of checks

            Sequence = block.Header.Sequence;
            Head = block.Header.Hash;
            Work = BigInteger.Parse(block.Header.Work);
        }
    }
}
