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

        public NotificationService(IHubContext<NotificationHub> hubContext, IUserRepo userRepo, ECommerceContext context)
        {
            _hubContext = hubContext;
            _userRepo = userRepo;
            _context = context;
        }

        public async Task AddNotificationToAllCustomers(string message)
        {
            await _hubContext.Clients.Group("Users").SendAsync("ReceiveNotification", message);

            var users = await _userRepo.GetCustomersAsync();
            foreach (var user in users)
            {
                var notification = new NotificationMessage
                {
                    UserId = user.UserId,
                    Title = "Notification",
                    MsgContent = message,
                    SendingDate = DateTime.Now,
                    Seen = false
                };
                _context.NotificationMessages.Add(notification);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationToAllAdmins(string message)
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
                    SendingDate = DateTime.Now,
                    Seen = false
                };
                _context.NotificationMessages.Add(notification);
            }
            await _context.SaveChangesAsync();
        }

        public async Task AddNotificationToCaller(int userId, string message)
        {
            userId = 11;
            await _hubContext.Clients.Group("Users").SendAsync("ReceiveNotification", message);

            var notification = new NotificationMessage
            {
                UserId = userId,
                Title = "Notification",
                MsgContent = message,
                SendingDate = DateTime.Now,
                Seen = false
            };
            _context.NotificationMessages.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationMessage>> GetAllMessages()
        {
            return await _context.NotificationMessages.Include(x => x.User).ToListAsync();
        }

        public async Task<List<NotificationMessage>> GetAllMessagesForUser(int userId)
        {
            return await _context.NotificationMessages.Include(x => x.User).Where(s => s.UserId == userId).ToListAsync();
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
