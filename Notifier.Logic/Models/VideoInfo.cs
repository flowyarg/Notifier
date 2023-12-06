namespace Notifier.Logic.Models
{
    public record VideoInfo(string Id, string OwnerId, string PlaylistId, string Title, string? Description, DateTimeOffset PublicationDate, TimeSpan Duration, string Url, string PreviewUrl);
}
