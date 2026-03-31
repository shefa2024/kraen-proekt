using Microsoft.AspNetCore.SignalR;

namespace LearnConnect.API.Hubs;

public class WhiteboardHub : Hub
{
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task DrawPath(string roomId, object pathData)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveDrawPath", pathData);
    }

    public async Task ClearCanvas(string roomId)
    {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveClearCanvas");
    }
}
