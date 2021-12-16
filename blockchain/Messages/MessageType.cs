namespace Blockchain.Messages
{
    public enum MessageType
    {
        Identity,
        Signal,
        SignalRequest,
        PeerList,
        PeerListRequest,
        CannotSatisfyRequest,
        Disconnecting,

        //Note,
        //Nullifier,
        NewBlock,
        //NewTransaction,
        //GetBlockHashes,
        //GetBlocks,
    }
}
