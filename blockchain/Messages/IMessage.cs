namespace Blockchain.Messages
{
    internal interface IMessage<T>
    {
        Message<T> CreateMessage();
    }
}
