namespace Aspire.ApiService.Services;

public class VideoHub : Hub {
    public async Task JoinRoom(string roomId) {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task SendData(byte[] data, string roomId) {
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveData", data);
    }
}