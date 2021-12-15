namespace Blockchain.Messages
{
    using System;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    internal class Message<T>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("type")]
        public InternalMessageType Type { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("payload")]
        public T Payload { get; }

        [JsonConstructor]
        public Message(
            InternalMessageType type,
            object payload)
        {
            Type = type;
            Payload = (T)payload;
        }

        public Message(
            InternalMessageType type,
            T payload)
        {
            if (!Enum.IsDefined(typeof(InternalMessageType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(InternalMessageType));

            Type = type;
            Payload = payload;
        }
    }
}
