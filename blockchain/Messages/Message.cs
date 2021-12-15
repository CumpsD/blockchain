namespace Blockchain.Messages
{
    using System;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    public class Message
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("type")]
        public InternalMessageType Type { get; }

        protected Message(
            InternalMessageType type)
        {
            if (!Enum.IsDefined(typeof(InternalMessageType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(InternalMessageType));

            Type = type;
        }
    }

    public class Message<T> : Message
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("payload")]
        public T Payload { get; }

        public Message(
            InternalMessageType type,
            T payload) : base(type)
        {
            Payload = payload;
        }
    }
}
