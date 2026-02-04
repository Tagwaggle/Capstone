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
    public List<RoomExit> CurrentRoomExits { get; set; } = new();
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
    public async Task SetCharacterAsync(int characterId)
    {
        CharacterId = characterId;
        if (RoomId.HasValue)
        {
            await MoveToRoom(RoomId.Value);
        }
        NotifyStateChange();
    }

    public async Task SetRoomAsync(int roomId)
    {
        RoomId = roomId;
        await MoveToRoom(roomId);
        NotifyStateChange();
    }

    public async Task SetCharacterAsync(PlayerCharacterDto character)
    {
        CharacterId = character.CharacterId;
        Pfile = character;
        RoomId = character.RoomId;

        // Hydrate the room state immediately
        if (RoomId.HasValue)
        {
            await MoveToRoom(RoomId.Value);
        }

        NotifyStateChange();
        Console.WriteLine($"[SessionState] Character set: {character.Name}");
    }

    public async Task MoveToRoom(int roomId)
    {
        CurrentRoom = await _roomService.GetRoomAsync(roomId);
        var mobs = await _characterService.GetNpcsInRoom(roomId);
        if (Pfile == null)
        {
            Console.WriteLine($"BUG: Moving with an inactive Pfile {roomId}");
            return;
        }
        Pfile.RoomId = roomId;
        CurrentRoomMobs = mobs.ToList();
        CurrentRoomContainers = (await _containerService.GetContainersInRoom(roomId)).ToList();
        CurrentRoomExits = (await _roomService.GetExitsForRoomAsync(roomId)).ToList();
        NotifyStateChange();
    }
    public async Task<bool> TryMoveAsync(string direction)
    {
        if (CurrentRoom == null) return false;

        var exit = await _roomService.GetExitAsync(CurrentRoom.RoomId, direction.ToLower());
        if (exit == null) return false;

        await MoveToRoom(exit.ToRoomId);
        return true;
    }

    private void NotifyStateChange() => Volatile.Read(ref OnChange)?.Invoke();
}
