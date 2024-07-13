using ECommereceApi.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Services.classes
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserRepo _userRepo;
        private readonly ECommerceContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IHubContext<NotificationHub> hubContext, IUserRepo userRepo, ECommerceContext context, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _userRepo = userRepo;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddNotificationToAllCustomers(string message, string hiddenLink = "#")
        {
            await _hubContext.Clients.Group("Users").SendAsync("ReceiveNotification", message, hiddenLink);
            await _hubContext.Clients.Group("Users").SendAsync("ReceiveNotificationCount", message, hiddenLink);

            var users = await _userRepo.GetCustomersAsync();
            foreach (var user in users)
            {
                var notification = new NotificationMessage
                {
                    UserId = user.UserId,
                    Title = "Notification",
                    MsgContent = message,
                    HiddenLink = hiddenLink,
                    SendingDate = DateTime.Now,
                    Seen = false
                };
                _context.NotificationMessages.Add(notification);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationToAllAdmins(string message,string hiddenLink = "#")
        {
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", message);

            var users = await _userRepo.GetAdminsAsync();
            foreach (var user in users)
            {
                var notification = new NotificationMessage
                {
                    UserId = user.UserId,
                    Title = "Notification",
                    MsgContent = message,
                    HiddenLink = hiddenLink,
                    SendingDate = DateTime.Now,
                    Seen = false
                };
                _context.NotificationMessages.Add(notification);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationToUser(int userId, string message,string hiddenLink="#")
        {
           
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);

            var notification = new NotificationMessage
            {
                UserId = userId,
                Title = "Notification",
                MsgContent = message,
                SendingDate = DateTime.Now,
                HiddenLink = hiddenLink,
                Seen = false
            };
            _context.NotificationMessages.Add(notification);
            await _context.SaveChangesAsync();
        }
        public async Task AddNotificationToCaller(string message, string hiddenLink = "#")
        {
            string userId = GetUserIdentifierAsync().Result;
            if(userId == null)
            {
                //throw new Exception("Invalid userId");
                return;
            }
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);

            var notification = new NotificationMessage
            {
                UserId = int.Parse(userId),
                Title = "Notification",
                MsgContent = message,
                HiddenLink = hiddenLink,
                SendingDate = DateTime.Now,
                Seen = false
            };
            _context.NotificationMessages.Add(notification);
            await _context.SaveChangesAsync();
        }
        private async Task<string?> GetUserIdentifierAsync()
        {
            return _httpContextAccessor.HttpContext?.Request.Query["userId"];
        }

        public async Task<List<NotificationMessage>> GetAllMessages()
        {
            return await _context.NotificationMessages.ToListAsync();
        }

        public async Task<List<NotificationMessage>> GetAllMessagesForUser(int userId)
        {
            return await _context.NotificationMessages.Include(x => x.User).Where(s => s.UserId == userId).OrderByDescending(x=>x.SendingDate).ToListAsync();
        }

        public async Task MarkAllAsRead(int userId)
        {
            var notifications = await _context.NotificationMessages.Where(s => s.UserId == userId).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.Seen = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            return await _context.NotificationMessages.CountAsync(s => s.UserId == userId && s.Seen == false);
        }
    }
}
