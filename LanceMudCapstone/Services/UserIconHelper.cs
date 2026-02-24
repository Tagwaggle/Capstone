using LanceMudCapstone.Enums;

namespace LanceMudCapstone.Services
{
    public class UserIconHelper
    {
        public static string GetIconPath(UserIcon icon)
        {
            return icon switch
            {
                UserIcon.Warrior => "/images/classes/warrior.svg",
                UserIcon.Mage => "/images/classes/mage.svg",
                UserIcon.Cleric => "/images/classes/cleric.svg",
                UserIcon.Paladin => "/images/classes/paladin.svg",
                UserIcon.Hunter => "/images/classes/hunter.svg",
                UserIcon.Rogue => "/images/classes/rogue.svg",
                UserIcon.Knight => "/images/classes/knight.svg",
                _ => "/images/classes/default.svg"
            };
        }
    }
}
