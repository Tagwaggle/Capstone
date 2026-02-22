using LanceMudCapstone.Enums;

namespace LanceMudCapstone.Services
{
    public class UserIconHelper
    {
        public static string GetIconPath(UserIcon icon)
        {
            return icon switch
            {
                UserIcon.Warrior => "/images/icons/warrior.svg",
                UserIcon.Mage => "/images/icons/mage.svg",
                UserIcon.Cleric => "/images/icons/cleric.svg",
                UserIcon.Paladin => "/images/icons/paladin.svg",
                UserIcon.Hunter => "/images/icons/hunter.svg",
                UserIcon.Rogue => "/images/icons/rogue.svg",
                UserIcon.Knight => "/images/icons/knight.svg",
                _ => "/images/icons/default.svg"
            };
        }
    }
}
