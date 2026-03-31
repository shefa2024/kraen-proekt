using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace LearnConnect.API.Hubs;

/// <summary>
/// SignalR Hub for WebRTC video call signaling.
/// Handles offer/answer/ICE candidate exchange between peers.
/// </summary>
public class VideoCallHub : Hub
{
    // roomId -> list of connectionIds
    private static readonly ConcurrentDictionary<string, List<string>> Rooms = new();
    // connectionId -> userId
    private static readonly ConcurrentDictionary<string, string> ConnectionUsers = new();

    /// <summary>
    /// Join a video call room (lessonId or reservationId based)
    /// </summary>
    public async Task JoinRoom(string roomId, string userId)
    {
        ConnectionUsers[Context.ConnectionId] = userId;

        Rooms.AddOrUpdate(roomId,
            new List<string> { Context.ConnectionId },
            (key, existing) =>
            {
                if (!existing.Contains(Context.ConnectionId))
                    existing.Add(Context.ConnectionId);
                return existing;
            });

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        // Notify others in the room that a new peer joined
        await Clients.OthersInGroup(roomId).SendAsync("PeerJoined", Context.ConnectionId, userId);

        // Tell the new joiner who's already in the room
        if (Rooms.TryGetValue(roomId, out var members))
        {
            var others = members.Where(c => c != Context.ConnectionId).ToList();
            await Clients.Caller.SendAsync("RoomMembers", others);
        }
    }

    /// <summary>
    /// Send WebRTC offer to a specific peer
    /// </summary>
    public async Task SendOffer(string targetConnectionId, string sdpOffer)
    {
        await Clients.Client(targetConnectionId).SendAsync("ReceiveOffer", Context.ConnectionId, sdpOffer);
    }

    /// <summary>
    /// Send WebRTC answer to a specific peer
    /// </summary>
    public async Task SendAnswer(string targetConnectionId, string sdpAnswer)
    {
        await Clients.Client(targetConnectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, sdpAnswer);
    }

    /// <summary>
    /// Send ICE candidate to a specific peer
    /// </summary>
    public async Task SendIceCandidate(string targetConnectionId, string candidate)
    {
        await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
    }

    /// <summary>
    /// Toggle mute state - broadcast to room
    /// </summary>
    public async Task ToggleMute(string roomId, bool isMuted)
    {
        await Clients.OthersInGroup(roomId).SendAsync("PeerMutedChanged", Context.ConnectionId, isMuted);
    }

    /// <summary>
    /// Toggle video state - broadcast to room
    /// </summary>
    public async Task ToggleVideo(string roomId, bool isVideoOff)
    {
        await Clients.OthersInGroup(roomId).SendAsync("PeerVideoChanged", Context.ConnectionId, isVideoOff);
    }

    /// <summary>
    /// End call - notify room and leave
    /// </summary>
    public async Task EndCall(string roomId)
    {
        await Clients.OthersInGroup(roomId).SendAsync("CallEnded", Context.ConnectionId);
        await LeaveRoom(roomId);
    }

    /// <summary>
    /// Leave room cleanup
    /// </summary>
    private async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        if (Rooms.TryGetValue(roomId, out var members))
        {
            members.Remove(Context.ConnectionId);
            if (members.Count == 0)
                Rooms.TryRemove(roomId, out _);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectionUsers.TryRemove(Context.ConnectionId, out _);

        // Remove from all rooms
        foreach (var room in Rooms)
        {
            if (room.Value.Contains(Context.ConnectionId))
            {
                await Clients.OthersInGroup(room.Key).SendAsync("PeerLeft", Context.ConnectionId);
                await LeaveRoom(room.Key);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
