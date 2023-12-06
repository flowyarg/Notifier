namespace Notifier.DataAccess.Model
{
    public abstract class Owner
    {
        public int Id { get; set; }
        public required string OwnerId { get; set; }
        public required string Url { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
