namespace LanceMudCapstone.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

public class MudHubClient : IAsyncDisposable
{
    private HubConnection? _hub;
    public event Action<string, string, string>? OnGlobalMessage;
    public event Action<string>? OnRoomEvent;

    public bool IsConnected =>
        _hub?.State == HubConnectionState.Connected;

    public async Task StartAsync(NavigationManager nav)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(nav.ToAbsoluteUri("/mudhub"))
            .WithAutomaticReconnect().Build();

        _hub.On<string, string, string>("ReceiveGlobalMessage", (user, msg, color) => OnGlobalMessage?.Invoke(user, msg, color));
        _hub.On<string>("ReceiveRoomEvent", (msg) => OnRoomEvent?.Invoke(msg));

        await _hub.StartAsync();
    }

    public async Task SendGlobalMessage(string user, string message, string color) =>
        await _hub!.SendAsync("SendGlobalMessage", user, message, color);

    public async Task JoinRoom(int roomId)
    {
        if (!IsConnected) return;
        await _hub!.SendAsync("JoinRoom", roomId.ToString());
    }

    public async Task LeaveRoom(int roomId)
    {
        if (!IsConnected) return;
        await _hub!.SendAsync("LeaveRoom", roomId.ToString());
    }

    public async Task AnnounceToRoom(int roomId, string message)
    {
        if (!IsConnected) return;
        await _hub!.SendAsync("AnnounceToRoom", roomId.ToString(), message);
    }

    public async ValueTask DisposeAsync()
    {
        if(_hub != null ) await _hub.DisposeAsync();
    }
}
