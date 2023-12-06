using System.ComponentModel.DataAnnotations.Schema;

namespace Notifier.DataAccess.Model
{
    public class Video
    {
        public int Id { get; set; }
        public string VideoId {  get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public TimeSpan Duration {  get; set; }
        public string Url { get; set; }
        public string PreviewUrl {  get; set; }


        public int OwnerId { get; set; }
        public int PlaylistId { get; set; }

        [ForeignKey($"{nameof(PlaylistId)}, {nameof(OwnerId)}")]
        public virtual Playlist Playlist { get; set; }
    }
}
