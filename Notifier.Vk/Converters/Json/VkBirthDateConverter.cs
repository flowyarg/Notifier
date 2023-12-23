using Newtonsoft.Json;
using Notifier.Vk.Models;

namespace Notifier.Vk.Converters.Json
{
    public class VkBirthDateConverter : JsonConverter<BirthDateInfo?>
    {
        public override BirthDateInfo? ReadJson(JsonReader reader, Type objectType, BirthDateInfo? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(BirthDateInfo))
            {
                return null;
            }
            var data = reader.Value as string;
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            var parts = data.Split('.');
            return parts switch
            {
                { Length: 3 } => new BirthDateInfo
                {
                    Day = int.Parse(parts[0]),
                    Month = int.Parse(parts[1]),
                    Year = int.Parse(parts[2]),
                },
                { Length: 2 } => new BirthDateInfo
                {
                    Day = int.Parse(parts[0]),
                    Month = int.Parse(parts[1]),
                },
                _ => null
            };
        }
        public override void WriteJson(JsonWriter writer, BirthDateInfo? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
