namespace Blockchain
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Messages;

    public class MessageConverter : JsonConverter<Message>
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            }
        };

        public override bool CanConvert(Type typeToConvert)
            => typeof(Message).IsAssignableFrom(typeToConvert);

        public override Message Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (readerClone.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string propertyName = readerClone.GetString();
            if (propertyName != "type")
                throw new JsonException();

            readerClone.Read();
            if (readerClone.TokenType != JsonTokenType.String)
                throw new JsonException();

            var typeDiscriminator = Enum.Parse<MessageType>(readerClone.GetString(), true);

            return typeDiscriminator switch
            {
                MessageType.Disconnecting
                    => JsonSerializer.Deserialize<Message<DisconnectingMessage>>(ref reader, _serializerOptions),

                MessageType.Identity
                    => JsonSerializer.Deserialize<Message<IdentityMessage>>(ref reader, _serializerOptions),

                MessageType.PeerList
                    => JsonSerializer.Deserialize<Message<PeerListMessage>>(ref reader, _serializerOptions),

                MessageType.PeerListRequest
                    => JsonSerializer.Deserialize<Message<PeerListRequestMessage>>(ref reader, _serializerOptions),

                MessageType.Signal
                    => JsonSerializer.Deserialize<Message<SignalMessage>>(ref reader, _serializerOptions),

                MessageType.SignalRequest
                    => JsonSerializer.Deserialize<Message<SignalRequestMessage>>(ref reader, _serializerOptions),

                MessageType.NewBlock
                    => JsonSerializer.Deserialize<Message<NewBlockMessage>>(ref reader, _serializerOptions),

                _ => JsonSerializer.Deserialize<Message>(ref reader, _serializerOptions),
            };
        }

        public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
