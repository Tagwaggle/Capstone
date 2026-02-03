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
    public bool CommandBox = true;
    public PlayerCharacterDto? Pfile {  get; set; }

    public Room? CurrentRoom { get; set; }
    public List<Container> CurrentRoomContainers { get; set; } = new();

    public event Action? OnChange;
    private readonly RoomService _roomService;
    private readonly CharacterService _characterService;
    private readonly ContainerService _containerService;
    public List<RoomNpcDto> CurrentRoomMobs { get; set; } = new();

    public SessionState(RoomService roomService,  CharacterService characterService, ContainerService containerService)
    {
        _roomService = roomService;
        _characterService = characterService;
        _containerService = containerService;
    }

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
    public void SetCharacter(PlayerCharacterDto character)
    {
        CharacterId = character.CharacterId;
        Pfile = character;
        RoomId = character.RoomId;
        NotifyStateChange();
        Console.WriteLine($"[SessionState] Character set: {character.Name}");
    }
    public async Task MoveToRoom(int roomId)
    {
        CurrentRoom = await _roomService.GetRoomAsync(roomId);
        var mobs = await _characterService.GetNpcsInRoom(roomId);
        CurrentRoomMobs = mobs.ToList();
        CurrentRoomContainers = (await _containerService.GetContainersInRoom(roomId)).ToList();
        NotifyStateChange();
    }

    private void NotifyStateChange() => Volatile.Read(ref OnChange)?.Invoke();
}
