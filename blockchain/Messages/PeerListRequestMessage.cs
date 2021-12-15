namespace Blockchain.Messages
{
    public class PeerListRequestMessage : IMessage<PeerListRequestMessage>
    {
        public Message<PeerListRequestMessage> CreateMessage()
            => new(InternalMessageType.PeerListRequest, this);
    }
}
