using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notifier.DataAccess.Model
{
    [PrimaryKey(nameof(Id), nameof(OwnerId))]
    public class Playlist
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string PlaylistId { get; set; }
        public required string Title { get; set; }
        public required string Url { get; set; }
        public bool IsBeingTracked { get; set; }
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }

        public virtual ICollection<Video> Videos { get; set; }
    }
}
