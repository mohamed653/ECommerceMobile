using ECommereceApi.Services.classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommereceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllMessagesForUser(int userId)
        {
            var messages = await _notificationService.GetAllMessagesForUser(userId);
            return Ok(messages);
        }

        [HttpPost("markAsRead/{msgId}")]
        public async Task<IActionResult> MarkMessageAsRead(int msgId)
        {
            await _notificationService.MarkMessageAsRead(msgId);
            return NoContent();
        }

        [HttpPost("sendToSelf")]
        public async Task<IActionResult> SendNotification(int userId, string title, string content)
        {
            await _notificationService.SendNotification(userId, title, content);
            return Ok();
        }

    }
}
