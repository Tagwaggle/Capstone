using LanceMudCapstone.DTOs;

namespace LanceMudCapstone.Services
{
    public interface IItemRepository
    {
        Task<IEnumerable<ItemDtoRaw>> GetInventory(int playerCharacterId);
        Task AddToInventory(int playerCharacterId, int itemId, int quantity);
        Task RemoveFromInventory(int playerCharacterId, int itemId, int quantity);
        Task<ItemDtoRaw?> GetItemForPlayerAsync(int playerCharacterId, int itemId);
    }
}
