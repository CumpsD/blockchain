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

            var typeDiscriminator = Enum.Parse<InternalMessageType>(readerClone.GetString(), true);
            return typeDiscriminator switch
            {
                InternalMessageType.Identity => JsonSerializer.Deserialize<Message<Identify>>(ref reader, _serializerOptions),
                InternalMessageType.PeerListRequest => JsonSerializer.Deserialize<Message<PeerListRequest>>(ref reader, _serializerOptions),
                _ => throw new JsonException()
            };
        }

        public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
            => throw new NotImplementedException();
    }
}
