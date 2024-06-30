using ECommereceApi.Services.classes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.HubConfig
{
    public class NotificationHub : Hub
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
            var userIdString = Context.GetHttpContext().Request.Query["userId"];
            if (!int.TryParse(userIdString, out int userIdentifier))
            {
                throw new Exception("Invalid userId");
            }

            var user = await _userRepo.GetUserAsync(userIdentifier);

            if (user.Role == RoleType.Admin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
            }

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

        public async Task SendNotificationToAllUser( string message)
        {
            await Clients.Group("Users").SendAsync("ReceiveNotification", message);
        }
    }
}
