namespace Blockchain.Messages
{
    using System;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    public class Message
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("type")]
        public MessageType Type { get; }

        protected Message(
            MessageType type)
        {
            if (!Enum.IsDefined(typeof(MessageType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(MessageType));

            Type = type;
        }
    }

    public class Message<T> : Message
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("payload")]
        public T Payload { get; }

        public Message(
            MessageType type,
            T payload) : base(type)
        {
            Payload = payload;
        }
    }
}
