using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LXGaming.TorrentAnalytics.Utilities.Json.Converters;

public abstract class StringListConverter(string? separator) : JsonConverter<ImmutableArray<string>> {

    public override ImmutableArray<string> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.String) {
            var value = reader.GetString();
            if (value != null) {
                return [..value.Split(separator)];
            }

            throw new JsonException($"String value '{value}' is not allowed.");
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing {typeToConvert.Name}.");
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<string> value, JsonSerializerOptions options) {
        writer.WriteStringValue(string.Join(separator, value));
    }
}