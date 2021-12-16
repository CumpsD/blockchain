namespace Blockchain.Messages
{
    public class PeerListRequestMessage : IMessage<PeerListRequestMessage>
    {
        public Message<PeerListRequestMessage> CreateMessage()
            => new(MessageType.PeerListRequest, this);
    }
}
