namespace Blockchain.Messages
{
    using System;
    using System.Text.Json.Serialization;

    internal class Identify : IMessage<Identify>
    {
        // TODO: Turn string into a string value type Identity later

        [JsonPropertyName("identity")]
        public string Identity { get; }

        [JsonPropertyName("name")]
        public string? Name { get; }

        [JsonPropertyName("version")]
        public int Version { get; }

        [JsonPropertyName("agent")]
        public string Agent { get; }

        [JsonPropertyName("port")]
        public int? Port { get; }

        [JsonPropertyName("head")]
        public string Head { get; }

        [JsonPropertyName("work")]
        public string Work { get; }

        [JsonPropertyName("sequence")]
        public int Sequence { get; }

        public Identify(
            string identity,
            string? name,
            int version,
            string agent,
            int? port,
            string head,
            string work,
            int sequence)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Name = name;
            Version = version;
            Agent = agent ?? throw new ArgumentNullException(nameof(agent));
            Port = port;
            Head = head ?? throw new ArgumentNullException(nameof(head));
            Work = work ?? throw new ArgumentNullException(nameof(work));
            Sequence = sequence;
        }

        public Message<Identify> CreateMessage()
            => new(InternalMessageType.Identify, this);
    }
}
