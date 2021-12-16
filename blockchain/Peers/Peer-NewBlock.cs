namespace Blockchain.Peers
{
    using Messages;
    using Microsoft.Extensions.Logging;

    public partial class Peer
    {
        private void HandleNewBlock(
            Message<NewBlockMessage> newBlockMessage)
        {
            //_logger.LogTrace(
            //    "[{Address,15}] New block from {Peer} - {Sequence}",
            //    Address,
            //    newBlockMessage.Payload.Block.Header.Sequence);
        }
    }
}
