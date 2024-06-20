using ECommereceApi.HubConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Services.classes
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ECommerceContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, ECommerceContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendNotification(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public async Task<List<NotificationMessage>> GetAllMessagesForUser(int userId)
        {
            return await _context.NotificationMessages
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SendingDate)
                .ToListAsync();
        }

        public async Task<int> GetNumberOfNotificationsForUser(int userId)
        {
            return await _context.NotificationMessages
                .Where(n => n.UserId == userId && !n.Seen)
                .CountAsync();
        }

        public async Task MarkMessageAsRead(int msgId)
        {
            var message = await _context.NotificationMessages.FindAsync(msgId);
            if (message != null)
            {
                message.Seen = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendNotification(int userId, string title, string content)
        {
            var notification = new NotificationMessage
            {
                UserId = userId,
                Title = title,
                MsgContent = content,
                SendingDate = DateOnly.FromDateTime(DateTime.Now),
                Seen = false
            };

            _context.NotificationMessages.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", title, content);
        }
    }
}
