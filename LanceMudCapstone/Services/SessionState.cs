using LanceMudCapstone.DTOs;
using LanceMudCapstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanceMudCapstone.Services;

public class SessionState
{
    private readonly LocalStorageService _localStorage;
    private readonly RoomService _roomService;
    private readonly CharacterService _characterService;
    private readonly ContainerService _containerService;

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
    public bool FirstLoadComplete { get; private set; } = false;
    public bool CharacterReady { get; private set; } = false;

    public event Action? OnChange;

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

    public async Task SetUserAsync(int userId, string username)
    {
        UserId = userId;
        UserName = username;

        await _localStorage.SetAsync("session.user", new UserSession(userId, username));
        var verify = await _localStorage.GetAsync<UserSession>("session.user");
        Console.WriteLine($"[SessionState] Verified localStorage: UserId={verify?.UserId}, UserName={verify?.UserName}");
        NotifyStateChange();
    }

    public async Task LogoutAsync()
    {
        UserId = null;
        UserName = null;
        CharacterId = null;
        RoomId = null;
        Pfile = null;
        CurrentRoom = null;
        CurrentRoomExits.Clear();
        CurrentRoomMobs.Clear();
        CurrentRoomContainers.Clear();

        await _localStorage.RemoveAsync("session.user");
        await _localStorage.RemoveAsync("session.character");

        CharacterReady = false;
        NotifyStateChange();
    }

    public async Task SetCharacterAsync(PlayerCharacterDto character)
    {
        CharacterReady = false; // reset while moving
        CharacterId = character.CharacterId;
        Pfile = character;
        RoomId = character.RoomId;

        if (RoomId.HasValue)
            await MoveToRoom(RoomId.Value);

        await _localStorage.SetAsync("session.character", new CharacterSession(CharacterId.Value, RoomId ?? 0));

        CharacterReady = true; // signal ready
        NotifyStateChange();
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

        CharacterReady = false;
        await _localStorage.RemoveAsync("session.character");
        NotifyStateChange();
    }

    public async Task MoveToRoom(int roomId)
    {
        CurrentRoom = await _roomService.GetRoomAsync(roomId);
        CurrentRoomMobs = (await _characterService.GetNpcsInRoom(roomId)).ToList();
        CurrentRoomContainers = (await _containerService.GetContainersInRoom(roomId)).ToList();
        CurrentRoomExits = (await _roomService.GetExitsForRoomAsync(roomId))?.ToList() ?? new List<RoomExit>();

        if (Pfile != null) Pfile.RoomId = roomId;
        RoomId = roomId;

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

                await TryHydrateCharacterAsync();
            }
        }
        catch
        {
            // Ignore localStorage read errors
        }

        Ready = true;
        NotifyStateChange();
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

    private void NotifyStateChange() => OnChange?.Invoke();

    public record UserSession(int UserId, string UserName);
    public record CharacterSession(int CharacterId, int RoomId);
}
