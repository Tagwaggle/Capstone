using LanceMudCapstone.DTOs;

namespace LanceMudCapstone.Services
{
    public interface IItemRepository
    {
        Task<IEnumerable<ItemDtoRaw>> GetInventory(int playerCharacterId);
    }
}
