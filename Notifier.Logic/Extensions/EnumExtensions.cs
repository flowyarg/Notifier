using Notifier.Logic.Models;
using Notifier.Vk.Models;

namespace Notifier.Logic.Extensions
{
    using System.Diagnostics;

    public static class EnumExtensions
    {
        [DebuggerStepThrough]
        public static VkOwnerType ToVk(this OwnerType ownerType) => ownerType switch
        {
            OwnerType.VkGroup => VkOwnerType.Group,
            OwnerType.VkUser => VkOwnerType.User,
            _ => throw new NotImplementedException(),
        };
    }
}
