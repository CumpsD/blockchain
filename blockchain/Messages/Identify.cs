namespace Blockchain.Messages
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A message by which a peer can identify itself to another.
    /// </summary>
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

        /// <summary>
        /// A message by which a peer can identify itself to another.
        /// </summary>
        /// <param name="identity">A base64-encoded 32-byte public key exposed to other peers on the network.</param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="agent"></param>
        /// <param name="port"></param>
        /// <param name="head"></param>
        /// <param name="work"></param>
        /// <param name="sequence"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
            => new(InternalMessageType.Identity, this);
    }
}
