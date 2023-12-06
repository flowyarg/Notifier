namespace Notifier.Logic.Models
{
    public abstract class Owner
    {
        public required string Id { get; set; }
        public required string Url {  get; set; }
        public abstract OwnerType OwnerType { get; }

        public abstract string DisplayName { get; }
    }
}
