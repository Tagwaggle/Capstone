using LanceMudCapstone.DTOs;

namespace LanceMudCapstone.Services
{
    public interface ICharacterService
    {
        Task<int> CreateCharacterAsync(PlayerCharacterDto dto);
        Task<PlayerCharacterDto> SavePlayer(PlayerCharacterDto saveState);
        Task<IEnumerable<PlayerCharacterDto>> GetCharactersByUserAsync(int userid);
        Task<IEnumerable<RoomNpcDto>> GetNpcsInRoom(int roomId);
        Task<IEnumerable<PlayerCharacterDto>> GetOnlineCharactersAsync();
    }

}
