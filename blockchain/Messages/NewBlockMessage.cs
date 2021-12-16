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
        public JsonBuffer[] Transactions { get; }

        public SerializedBlock(
            SerializedBlockHeader header,
            JsonBuffer[] transactions)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        }
    }

    public class SerializedBlockHeader
    {
        [JsonPropertyName("sequence")]
        public int Sequence { get; }

        [JsonPropertyName("previousBlockHash")]
        public string PreviousBlockHash { get; }

        [JsonPropertyName("noteCommitment")]
        public NoteCommitment NoteCommitment { get; set; }

        [JsonPropertyName("nullifierCommitment")]
        public NullifierCommitment NullifierCommitment { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; }

        [JsonPropertyName("randomness")]
        public long Randomness { get; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; }

        [JsonPropertyName("minersFee")]
        public string MinersFee { get; }

        [JsonPropertyName("work")]
        public string Work { get; }

        [JsonPropertyName("hash")]
        public string Hash { get; }

        [JsonPropertyName("graffiti")]
        public string Graffiti { get; }

        public SerializedBlockHeader(
            int sequence,
            string previousBlockHash,
            NoteCommitment noteCommitment,
            NullifierCommitment nullifierCommitment,
            string target,
            long randomness,
            long timestamp,
            string minersFee,
            string work,
            string hash,
            string graffiti)
        {
            Sequence = sequence;
            PreviousBlockHash = previousBlockHash ?? throw new ArgumentNullException(nameof(previousBlockHash));
            NoteCommitment = noteCommitment ?? throw new ArgumentNullException(nameof(noteCommitment));
            NullifierCommitment = nullifierCommitment ?? throw new ArgumentNullException(nameof(nullifierCommitment));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Randomness = randomness;
            Timestamp = timestamp;
            MinersFee = minersFee;
            Work = work ?? throw new ArgumentNullException(nameof(work));
            Hash = hash ?? throw new ArgumentNullException(nameof(hash));
            Graffiti = graffiti ?? throw new ArgumentNullException(nameof(graffiti));
        }
    }

    public class NoteCommitment
    {
        [JsonPropertyName("commitment")]
        public JsonBuffer Commitment { get; }

        [JsonPropertyName("size")]
        public int Size { get; }

        public NoteCommitment(
            JsonBuffer commitment,
            int size)
        {
            Commitment = commitment ?? throw new ArgumentNullException(nameof(commitment));
            Size = size;
        }
    }

    public class NullifierCommitment
    {
        [JsonPropertyName("commitment")]
        public string Commitment { get; }

        [JsonPropertyName("size")]
        public int Size { get; }

        public NullifierCommitment(
            string commitment,
            int size)
        {
            Commitment = commitment ?? throw new ArgumentNullException(nameof(commitment));
            Size = size;
        }
    }

    public class JsonBuffer
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("data")]
        public int[] Data { get; }

        public JsonBuffer(
            string type,
            int[] data)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
