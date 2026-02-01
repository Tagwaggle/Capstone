using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services;

public class SessionState
{
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public int? CharacterId { get; private set; }
    public int? RoomId { get; private set; }
    public bool IsLoggedIn => UserId.HasValue;

    public PlayerCharacterDto Pfile = new PlayerCharacterDto();
    

    public event Action? OnChange;

    public void SetUser(int userId, string username)
    {

        UserId = userId; UserName = username; NotifyStateChange();
        Console.WriteLine($"[SessionState] User set: {UserId} - {UserName}");
    }
    public void Logout()
    {
        UserId = null; UserName = null; CharacterId = null; RoomId = null; NotifyStateChange(); 
    }
    public void SetCharacter(int characterId)
    {
        CharacterId = characterId;
        NotifyStateChange();
    }
    public void SetRoom(int roomId)
    {
        RoomId = roomId;
        NotifyStateChange();
    }  
    private void NotifyStateChange() => Volatile.Read(ref OnChange)?.Invoke();
}
