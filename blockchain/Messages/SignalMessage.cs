namespace Blockchain.Messages
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// A message used to indicate to a peer that we want them to
    /// initiate signaling with us.This is most often used when
    /// we discover a peer through another peer but need to indicate
    /// to them through a brokering peer to connect to us via webrtc.
    /// </summary>
    public class SignalMessage : IMessage<SignalMessage>
    {
        [JsonPropertyName("sourceIdentity")]
        public string SourceIdentity { get; }

        [JsonPropertyName("destinationIdentity")]
        public string DestinationIdentity { get; }

        [JsonPropertyName("nonce")]
        public string Nonce { get; }

        [JsonPropertyName("signal")]
        public string Signal { get; }

        public SignalMessage(
            string sourceIdentity,
            string destinationIdentity,
            string nonce,
            string signal)
        {
            SourceIdentity = sourceIdentity;
            DestinationIdentity = destinationIdentity;
            Nonce = nonce;
            Signal = signal;
        }

        public Message<SignalMessage> CreateMessage()
            => new(MessageType.Signal, this);
    }
}
