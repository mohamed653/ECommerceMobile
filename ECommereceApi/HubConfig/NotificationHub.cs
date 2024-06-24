using ECommereceApi.Services.classes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.HubConfig
{
    public class NotificationHub:Hub
    {
        private readonly NotificationService _notificationService;
        private readonly IUserRepo _userRepo;
        public NotificationHub(NotificationService notificationService, IUserRepo userRepo)
        {
            _notificationService = notificationService;
            _userRepo = userRepo;
        }
        public override async Task OnConnectedAsync()
        {
            int userIdentifier = int.Parse(Context.UserIdentifier);
            var user = _userRepo.GetUserAsync(userIdentifier).Result;

            if (user.Role == RoleType.Admin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
            }

            await Clients.All.SendAsync("ReceiveNotification", $"A user: {userIdentifier} has connected");

            await base.OnConnectedAsync();
        }

        public async Task MarkAllNotificationAsRead()
        {
            var userId = int.Parse(Context.UserIdentifier);
            await _notificationService.MarkAllAsRead(userId);
        }

        public async Task GetNotificationCount()
        {
            var userId = int.Parse(Context.UserIdentifier);
            var count = await _notificationService.GetUnreadCount(userId);
            await Clients.Caller.SendAsync("ReceiveNotificationCount", count);
        }

        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
