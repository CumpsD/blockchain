namespace Blockchain.Messages
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A message used to indicate to a peer that we want them to
    /// initiate signaling with us.This is most often used when
    /// we discover a peer through another peer but need to indicate
    /// to them through a brokering peer to connect to us via webrtc.
    /// </summary>
    public class SignalRequestMessage : IMessage<SignalRequestMessage>
    {
        [JsonPropertyName("sourceIdentity")]
        public string SourceIdentity { get; }

        [JsonPropertyName("destinationIdentity")]
        public string DestinationIdentity { get; }

        public SignalRequestMessage(
            string sourceIdentity,
            string destinationIdentity)
        {
            SourceIdentity = sourceIdentity ?? throw new ArgumentNullException(nameof(sourceIdentity));
            DestinationIdentity = destinationIdentity ?? throw new ArgumentNullException(nameof(destinationIdentity));
        }

        public Message<SignalRequestMessage> CreateMessage()
            => new(InternalMessageType.SignalRequest, this);
    }
}
