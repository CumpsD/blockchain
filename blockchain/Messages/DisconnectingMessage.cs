namespace Blockchain.Messages
{
    using System;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    public enum DisconnectingReason
    {
        ShuttingDown = 0,
        Congested = 1,
    }

    /// <summary>
    /// A message used to indicate to a peer that we want them to
    /// initiate signaling with us.This is most often used when
    /// we discover a peer through another peer but need to indicate
    /// to them through a brokering peer to connect to us via webrtc.
    /// </summary>
    public class DisconnectingMessage : IMessage<DisconnectingMessage>
    {
        [JsonPropertyName("sourceIdentity")]
        public string SourceIdentity { get; }

        /// <summary>
        /// Can be null if we're sending the message to an unidentified Peer
        /// </summary>
        [JsonPropertyName("destinationIdentity")]
        public string? DestinationIdentity { get; }

        [JsonPropertyName("reason")]
        public DisconnectingReason Reason { get; }

        [JsonPropertyName("disconnectUntil")]
        public int DisconnectUntil { get; }

        public DisconnectingMessage(
            string sourceIdentity,
            string? destinationIdentity,
            DisconnectingReason reason,
            int disconnectUntil)
        {
            if (!Enum.IsDefined(typeof(DisconnectingReason), reason))
                throw new InvalidEnumArgumentException(nameof(reason), (int)reason, typeof(DisconnectingReason));

            SourceIdentity = sourceIdentity ?? throw new ArgumentNullException(nameof(sourceIdentity));
            DestinationIdentity = destinationIdentity;
            Reason = reason;
            DisconnectUntil = disconnectUntil;
        }

        public Message<DisconnectingMessage> CreateMessage()
            => new(InternalMessageType.Disconnecting, this);
    }
}
