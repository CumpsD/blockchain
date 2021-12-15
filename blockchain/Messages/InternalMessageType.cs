namespace Blockchain.Messages
{
    internal enum InternalMessageType
    {
        Identity,
        Signal,
        SignalRequest,
        PeerList,
        PeerListRequest,
        CannotSatisfyRequest,
        Disconnecting,
    }
}
