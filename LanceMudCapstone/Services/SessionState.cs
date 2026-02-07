using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services;

public class SessionState
{
    private readonly LocalStorageService _localStorage;
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public int? CharacterId { get; private set; }
    public int? RoomId { get; private set; }
    public bool IsLoggedIn => UserId.HasValue;
    public bool CommandBox = true;
    public PlayerCharacterDto? Pfile { get; private set; }

    public Room? CurrentRoom { get; private set; }
    public List<Container> CurrentRoomContainers { get; private set; } = new();
    public List<RoomExit> CurrentRoomExits { get; private set; } = new();
    public List<RoomNpcDto> CurrentRoomMobs { get; private set; } = new();

    public bool Ready { get; private set; } = false;

    public event Action? OnChange;

    private readonly RoomService _roomService;
    private readonly CharacterService _characterService;
    private readonly ContainerService _containerService;
    //private readonly IJSRuntime _js;

    public SessionState(
        RoomService roomService,
        CharacterService characterService,
        ContainerService containerService,
        LocalStorageService localStorage )
        //IJSRuntime js)
    {
        _roomService = roomService;
        _characterService = characterService;
        _containerService = containerService;
        _localStorage = localStorage;
        //_js = js;
    }

    public async Task SetUser(int userId, string username)
    {
        UserId = userId;
        UserName = username;

        await _localStorage.SetAsync("session.user", new
        {
            UserId = userId,
            UserName = username
        });

        NotifyStateChange();
        //Console.WriteLine($"[SessionState] User set: {UserId} - {UserName}");
    }

    public async Task Logout()
    {
        UserId = null;
        UserName = null;
        CharacterId = null;
        RoomId = null;

        await _localStorage.RemoveAsync("session.user");
        await _localStorage.RemoveAsync("session.character");

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

        // Save to localStorage
//        await _js.InvokeVoidAsync("localStorage.setItem", "CharacterId", CharacterId.ToString());

        Console.WriteLine($"[SessionState] Character set: {character.Name}");
    }
    public async Task ClearCharacterAsync()
    {
        CharacterId = null;
        Pfile = null;
        RoomId = null;
        CurrentRoom = null;
        CurrentRoomExits.Clear();
        CurrentRoomMobs.Clear();
        CurrentRoomContainers.Clear();

        await _localStorage.RemoveAsync("session.character");
        NotifyStateChange();
    }

    public async Task SetRoomAsync(int roomId)
    {
        RoomId = roomId;
        await MoveToRoom(roomId);
        NotifyStateChange();
    }

    public async Task MoveToRoom(int roomId)
    {
        CurrentRoom = await _roomService.GetRoomAsync(roomId);
        CurrentRoomMobs = (await _characterService.GetNpcsInRoom(roomId)).ToList();
        CurrentRoomContainers = (await _containerService.GetContainersInRoom(roomId)).ToList();

        try
        {
            CurrentRoomExits = (await _roomService.GetExitsForRoomAsync(roomId))?.ToList() ?? new List<RoomExit>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching exits for room {roomId}: {ex.Message}");
            CurrentRoomExits = new List<RoomExit>();
        }

        if (Pfile != null) Pfile.RoomId = roomId;
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

    public async Task InitializeAsync()
    {
        try
        {
            var user = await _localStorage.GetAsync<UserSession>("session.user");
            if (user != null)
            {
                UserId = user.UserId;
                UserName = user.UserName;
            }

            // Restore character
            if (UserId.HasValue)
            {
                var storedChar = await _localStorage.GetAsync<CharacterSession>("session.character");
                if (storedChar != null)
                {
                    await HydrateCharacterAsync(storedChar.CharacterId);
                }
            }
        }
        catch
        {
            // Ignore errors reading localStorage
        }

        Ready = true;
        NotifyStateChange();
    }

    private async Task HydrateCharacterAsync(int characterId)
    {
        if (!UserId.HasValue) return;

        Pfile = await _characterService.LoadCharacterAsync(UserId.Value, characterId);
        if (Pfile != null && Pfile.RoomId.HasValue)
        {
            RoomId = Pfile.RoomId;
            await MoveToRoom(RoomId.Value);
        }

    }

    public async Task TryHydrateAsync()
    {
        var user = await _localStorage.GetAsync<UserSession>("session.user");

        if (user == null) return;

        UserId = user.UserId;
        UserName = user.UserName;

        NotifyStateChange();
    }
    public async Task TryHydrateCharacterAsync()
    {
        if (UserId == null) return;

        var stored = await _localStorage.GetAsync<CharacterSession>("session.character");
        if (stored == null) return;

        var character = await _characterService.GetCharactersByUserAsync(UserId.Value);
        var selected = character.FirstOrDefault(c => c.CharacterId == stored.CharacterId);

        if (selected == null) return;

        await SetCharacterAsync(selected);
    }

    private record UserSession(int UserId, string UserName);
    private record CharacterSession(int CharacterId, int RoomId);

    private void NotifyStateChange() => Volatile.Read(ref OnChange)?.Invoke();
}
