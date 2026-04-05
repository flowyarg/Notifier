using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notifier.DataAccess.Converters;
using Notifier.DataAccess.Model;

namespace Notifier.DataAccess
{
    public class NotifierDbContext : DbContext
    {
        public NotifierDbContext(DbContextOptions options) : base(options)
        { }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<PlaylistSubscription> Subscriptions { get; set; }
        public virtual DbSet<AccessTokenInfo>  AccessTokens { get; set; }
        public virtual DbSet<MatrixAccessToken>  MatrixAccessTokens { get; set; }
        public virtual DbSet<VkVideoAccessToken>  VkVideoAccessTokens { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<DateTimeOffset>()
                .HaveConversion<DateTimeOffsetConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Owner>()
                .UseTptMappingStrategy();
            
            modelBuilder
                .Entity<AccessTokenInfo>()
                .UseTptMappingStrategy();

            modelBuilder
                .Entity<Video>()
                .Property(video => video.Duration)
                .HasConversion<TimeSpanToStringConverter>();
        }
    }
}
