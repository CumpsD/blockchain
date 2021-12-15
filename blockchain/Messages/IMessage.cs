namespace Blockchain.Messages
{
    public interface IMessage<T>
    {
        Message<T> CreateMessage();
    }
}
