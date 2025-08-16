using Microsoft.AspNetCore.SignalR;

namespace Hirsch.Ws
{
    public class FavoritesHub : Hub
    {
        public async Task JoinGroup(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
            Console.WriteLine($"Usuario {username} se unió al grupo");
        }

        public async Task LeaveGroup(string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, username);
            Console.WriteLine($"Usuario {username} salió del grupo");
        }
    }
}
