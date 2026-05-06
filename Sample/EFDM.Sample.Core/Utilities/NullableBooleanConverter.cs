using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EFDM.Sample.Core.Utilities;

public class NullableBooleanConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt32(out int value) ? (bool?)(value == 1) : null,
            JsonTokenType.Null => null,
            _ => throw new JsonException($"Не удалось преобразовать токен {reader.TokenType} в bool?")
        };
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
