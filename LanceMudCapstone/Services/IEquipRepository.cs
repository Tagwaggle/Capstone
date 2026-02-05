using LanceMudCapstone.DTOs;
using LanceMudCapstone.Enums;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services
{
    public interface IEquipRepository
    {
        Task<IEnumerable<EquippedItemRaw>> GetEquipped(int playerCharacterId);
        Task<EquippedItem?> GetEquippedItemAsync(int playerId, EquipSlot slot);
        Task EquipAsync(int playerId, int itemId, EquipSlot slot);
        Task UnequipAsync(int equippedItemId);
    }
}
