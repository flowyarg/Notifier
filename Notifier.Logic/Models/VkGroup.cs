namespace Notifier.Logic.Models
{
    public class VkGroup : Owner
    {
        public required string Name { get; set; }
        public override OwnerType OwnerType => OwnerType.VkGroup;
        public override string DisplayName => Name;
    }
}
