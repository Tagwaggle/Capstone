namespace LanceMudCapstone.Hubs;
using Microsoft.AspNetCore.SignalR;

public class MudHub : Hub
{
    public async Task JoinRoom(string roomId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
    public async Task LeaveRoom(string roomId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
    public async Task SendGlobalMessage(string user, string message, string color) =>
        await Clients.All.SendAsync("ReceiveGlobalMessage", user, message, color);
    public async Task SendRoomMessage(string roomId, string message) =>
        await Clients.Group($"room_{roomId}").SendAsync("ReceiveRoomMessage", message);

}
