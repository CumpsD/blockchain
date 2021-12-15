namespace Blockchain.Messages
{
    using System;
    using System.ComponentModel;
    using System.Text.Json.Serialization;

    internal class Message<T>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public InternalMessageType Type { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("payload")]
        public T Payload { get; }

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
