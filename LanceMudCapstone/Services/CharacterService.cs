using Dapper;
using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services;

public class CharacterService
{
    private readonly DbHelper _db;

    public CharacterService(DbHelper db)
    {
        _db = db;
    }

    public Task<int> CreateCharacterAsync(CreateCharacterDto dto)
        => Task.FromResult(0);

    public Task<bool> UpdateCharacterAsync(UpdateCharacterDto dto)
        => Task.FromResult(true);

    public Task<CharacterDto?> GetCharacterByIdAsync(int id)
        => Task.FromResult<CharacterDto?>(null);
}
