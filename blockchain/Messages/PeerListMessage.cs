namespace Blockchain.Messages
{
    using System;
    using System.Text.Json.Serialization;

    public class PeerListMessage : IMessage<PeerListMessage>
    {
        [JsonPropertyName("connectedPeers")]
        public ConnectedPeer[] ConnectedPeers { get; }

        public PeerListMessage(ConnectedPeer[] connectedPeers)
        {
            ConnectedPeers = connectedPeers;
        }

        public Message<PeerListMessage> CreateMessage()
            => new(MessageType.PeerList, this);
    }

    public class ConnectedPeer
    {
        [JsonPropertyName("identity")]
        public string Identity { get; }

        [JsonPropertyName("name")]
        public string? Name { get; }

        [JsonPropertyName("address")]
        public string? Address { get; }

        [JsonPropertyName("port")]
        public int? Port { get; }

        public ConnectedPeer(
            string identity,
            string? name,
            string? address,
            int? port)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Name = name;
            Address = address;
            Port = port;
        }
    }
}
