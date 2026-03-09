using LanceMudCapstone.DTOs;
using LanceMudCapstone.Enums;
using LanceMudCapstone.Models;

namespace LanceMudCapstone.Services;


public class SessionState
{
    private readonly LocalStorageService _localStorage;
    private readonly RoomService _roomService;
    private readonly CharacterService _characterService;
    private readonly ContainerService _containerService;

    public int? UserId { get; private set; }
    public string LastEventMessage { get; set; } = string.Empty;
    public List<string> EventLog { get; set; } = new();
    public string? UserName { get; private set; }
    public int? CharacterId { get; private set; }
    public int? RoomId { get; private set; }
    public bool IsOnline { get; private set; }
    public PlayerCharacterDto? CurrentMob { get; set; }


    public bool IsLoggedIn => UserId.HasValue;
    public bool CommandBox = true;
    public PlayerCharacterDto? Pfile { get; private set; }

    public Room? CurrentRoom { get; private set; }
    public List<Container> CurrentRoomContainers { get; set; } = new();
    public List<RoomExit> CurrentRoomExits { get; set; } = new();
    public List<RoomNpcDto> CurrentRoomMobs { get; set; } = new();
    public List<RoomPcDto> CurrentRoomPcs { get; set; } = new();

    public bool Ready { get; private set; } = false;
    public bool FirstLoadComplete { get; private set; } = false;
    public bool CharacterReady { get; private set; } = false;
    public bool InCombat { get; set; }
    public bool ShowLeftPanel { get; set; }
    public bool ShowRightPanel { get; set; }
    public UserIcon UserIcon { get; set; } = UserIcon.None;


    //public event Action? OnChange;
    public event Func<Task>? OnChange;

    public SessionState(RoomService roomService,
                        CharacterService characterService,
                        ContainerService containerService,
                        LocalStorageService localStorage)
    {
        _roomService = roomService;
        _characterService = characterService;
        _containerService = containerService;
        _localStorage = localStorage;
    }

    public async Task SetUserAsync(int userId, string username, UserIcon icon)
    {
        UserId = userId;
        UserName = username;
        UserIcon = icon;

        IsOnline = true;
        Ready = true;
        await _localStorage.SetAsync("session.user", new UserSession(userId, username));
        var verify = await _localStorage.GetAsync<UserSession>("session.user");
        Console.WriteLine($"[SessionState] Verified localStorage: UserId={verify?.UserId}, UserName={verify?.UserName}");
        await NotifyStateChange();
    }
    public async Task SetCombat(bool value)
    {
        InCombat = value;
        await NotifyStateChange();
    }
    public async Task SetEvent(string message)
    {
        LastEventMessage = message;
        await NotifyStateChange();
    }

    public async Task PushEvent(string message)
    {
        EventLog.Add(message);
        if (EventLog.Count > 10) EventLog.RemoveAt(0);

        await NotifyStateChange();
    }
    public async Task LogoutAsync()
    {
        if (CharacterId.HasValue)
            await _characterService.SetOffline(CharacterId.Value);

        UserId = null;
        UserIcon = UserIcon.None;
        UserName = null;
        CharacterId = null;
        RoomId = null;
        Pfile = null;
        CurrentRoom = null;
        IsOnline = false;
        CurrentRoomExits.Clear();
        CurrentRoomMobs.Clear();
        CurrentRoomPcs.Clear();
        CurrentRoomContainers.Clear();

        await _localStorage.RemoveAsync("session.user");
        await _localStorage.RemoveAsync("session.character");

        CharacterReady = false;
        await NotifyStateChange();
    }
    public async Task RefreshCharacterAsync()
    {
        if (CharacterId == null) return;

        var updated = await _characterService.GetCharacterByIdAsync(CharacterId.Value);
        if (updated != null)
        {
            Pfile = updated;
            await NotifyStateChange();
        }
    }

    public async Task SetCharacterAsync(PlayerCharacterDto character)
    {
        CharacterReady = false; // reset while moving
        CharacterId = character.CharacterId;
        IsOnline = true;
        await _characterService.SetOnline(character.CharacterId);
        Pfile = character;
        RoomId = character.RoomId;

        if (RoomId.HasValue)
            await MoveToRoom(RoomId.Value);

        await _localStorage.SetAsync("session.character", new CharacterSession(CharacterId.Value, RoomId ?? 0));

        CharacterReady = true; // signal ready
        await NotifyStateChange();
    }

    public async Task ClearCharacterAsync()
    {
        if (CharacterId.HasValue)
            await _characterService.SetOffline(CharacterId.Value);
        CharacterId = null;
        Pfile = null;
        RoomId = null;
        CurrentRoom = null;
        IsOnline = false;
        CurrentRoomExits.Clear();
        CurrentRoomMobs.Clear();
        CurrentRoomPcs.Clear();
        CurrentRoomContainers.Clear();
        Console.WriteLine("User ID Logged in" + UserId);
        //CharacterReady = false;
        //await _localStorage.RemoveAsync("session.character");
        await NotifyStateChange();
    }

    public async Task MoveToRoom(int roomId)
    {
        if (!(CurrentRoom != null && CurrentRoom.RoomId == roomId))
            EventLog.Clear();
        CurrentRoom = await _roomService.GetRoomAsync(roomId);
        CurrentRoomMobs = (await _characterService.GetNpcsInRoom(roomId)).ToList();
        CurrentRoomPcs = (await _characterService.GetPCsInRoom(roomId)).ToList();
        CurrentRoomContainers = (await _containerService.GetContainersInRoom(roomId)).ToList();
        CurrentRoomExits = (await _roomService.GetExitsForRoomAsync(roomId))?.ToList() ?? new List<RoomExit>();

        if (Pfile != null) Pfile.RoomId = roomId;
        if (Pfile != null) await _characterService.SavePlayer(Pfile);
        RoomId = roomId;

        // _mudHubClient.NotifyRoomChange(roomId); // Notify the hub of the room change

        await NotifyStateChange();
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
            try
            {
                bool check;
                check = user != null;
                Console.WriteLine($"Bool Check: {check} {user.UserName} {user.UserId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Log Error: " + e);
            }
            if (user != null)
            {
                UserId = user.UserId;
                UserName = user.UserName;

                await TryHydrateCharacterAsync();
            }
        }
        catch
        {
            // Ignore localStorage read errors
        }

        Ready = true;
        await NotifyStateChange();
    }

    public async Task TryHydrateCharacterAsync(int characterId)
    {
        CharacterReady = false;

        if (UserId == null)
            return;

        PlayerCharacterDto? selected = null;

        var stored = await _localStorage.GetAsync<CharacterSession>("session.character");
        if (stored?.CharacterId == characterId)
        {
            var characters = await _characterService.GetCharactersByUserAsync(UserId.Value);
            selected = characters.FirstOrDefault(c => c.CharacterId == characterId);
        }

        if (selected == null)
        {
            selected = await _characterService.LoadCharacterAsync(UserId.Value, characterId);
        }

        if (selected != null)
        {
            await SetCharacterAsync(selected); // sets CharacterReady = true
            Console.WriteLine($"[SessionState] Hydrated character via ID: {selected.Name}");
        }
        else
        {
            Console.WriteLine($"[SessionState] Character {characterId} not found for UserId={UserId}");
        }
    }

    public async Task TryHydrateCharacterAsync()
    {
        CharacterReady = false;

        if (!UserId.HasValue) return;

        var stored = await _localStorage.GetAsync<CharacterSession>("session.character");
        if (stored == null) return;

        var characters = await _characterService.GetCharactersByUserAsync(UserId.Value);
        var selected = characters.FirstOrDefault(c => c.CharacterId == stored.CharacterId);
        if (selected == null) return;

        await SetCharacterAsync(selected); // sets CharacterReady = true
    }

    public async Task EnsureInitializedAsync()
    {
        if (!Ready)
        {
            await InitializeAsync();
            await TryHydrateCharacterAsync();
        }

        FirstLoadComplete = true;
    }



    //    public void NotifyStateChange() => OnChange?.Invoke();

    public async Task NotifyStateChange()
    {
        Console.WriteLine($"[NotifyStateChange] START {DateTime.UtcNow:HH:mm:ss.fff}");
        if (OnChange != null)
            await OnChange.Invoke();
        Console.WriteLine($"[NotifyStateChange] END {DateTime.UtcNow:HH:mm:ss.fff}");
    }
    /*public async Task NotifyStateChange()
    {
        if (OnChange != null)
            await OnChange.Invoke();
    }*/

    public record UserSession(int UserId, string UserName);
    public record CharacterSession(int CharacterId, int RoomId);
}
