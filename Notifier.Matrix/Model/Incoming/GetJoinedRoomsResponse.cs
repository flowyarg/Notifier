using Newtonsoft.Json;

namespace Notifier.Matrix.Model.Incoming;

public class GetJoinedRoomsResponse
{
    [JsonProperty("joined_rooms")] 
    public required string[] Rooms { get; set; }
}