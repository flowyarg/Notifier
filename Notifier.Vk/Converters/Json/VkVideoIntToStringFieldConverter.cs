namespace Notifier.Vk.Converters.Json;

using Newtonsoft.Json;

public class VkVideoIntToStringFieldConverter : JsonConverter<string>
{
    public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (objectType != typeof(string))
        {
            return null;
        }

        if (reader.ValueType == typeof(string))
            return reader.Value as string;

        if (reader.ValueType == typeof(int))
            return ((int)reader.Value!).ToString();
        
        if (reader.ValueType == typeof(long))
            return ((long)reader.Value!).ToString();
        
        throw new JsonSerializationException($"Cannot convert \"{reader.Value}\" into {objectType}");
    }
}