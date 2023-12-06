namespace Notifier.Logic.Models
{
    public class VkUser : Owner
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public override OwnerType OwnerType => OwnerType.VkUser;
        public override string DisplayName => $"{FirstName} {LastName}";
    }
}
