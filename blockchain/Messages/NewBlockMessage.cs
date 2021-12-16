namespace Blockchain.Messages
{
    using System;
    using System.Text.Json.Serialization;

    public class NewBlockMessage : IMessage<NewBlockMessage>
    {
        [JsonPropertyName("block")]
        public SerializedBlock Block { get; }

        public NewBlockMessage(SerializedBlock block)
        {
            Block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public Message<NewBlockMessage> CreateMessage()
            => new(MessageType.NewBlock, this);
    }

    public class SerializedBlock
    {
        [JsonPropertyName("header")]
        public SerializedBlockHeader Header { get; }

        [JsonPropertyName("transactions")]
        public byte[] Transactions { get; }

        public SerializedBlock(
            SerializedBlockHeader header,
            byte[] transactions)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        }
    }

    public class SerializedBlockHeader
    {
        [JsonPropertyName("sequence")]
        public long Sequence { get; }

        [JsonPropertyName("previousBlockHash")]
        public string PreviousBlockHash { get; }

        [JsonPropertyName("target")]
        public string Target { get; }

        [JsonPropertyName("randomness")]
        public long Randomness { get; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; }

        [JsonPropertyName("minersFee")]
        public long MinersFee { get; }

        [JsonPropertyName("work")]
        public string Work { get; }

        [JsonPropertyName("hash")]
        public string Hash { get; }

        [JsonPropertyName("graffiti")]
        public string Graffiti { get; }

        public SerializedBlockHeader(
            long sequence,
            string previousBlockHash,
            string target,
            long randomness,
            long timestamp,
            long minersFee,
            string work,
            string hash,
            string graffiti)
        {
            Sequence = sequence;
            PreviousBlockHash = previousBlockHash ?? throw new ArgumentNullException(nameof(previousBlockHash));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Randomness = randomness;
            Timestamp = timestamp;
            MinersFee = minersFee;
            Work = work ?? throw new ArgumentNullException(nameof(work));
            Hash = hash ?? throw new ArgumentNullException(nameof(hash));
            Graffiti = graffiti ?? throw new ArgumentNullException(nameof(graffiti));
        }
    }
}
