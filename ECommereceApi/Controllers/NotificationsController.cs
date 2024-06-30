
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ECommereceApi.Services.classes;
using ECommereceApi.DTOs.Notification;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly NotificationService _notificationService;
    private readonly IMapper _mappper;

    public NotificationController(ILogger<NotificationController> logger, NotificationService notificationService, IMapper mappper)
    {
        _logger = logger;
        _notificationService = notificationService;
        _mappper = mappper;
    }


    [HttpGet("GetAllNotifications/{userId}")]
    public async Task<IActionResult> GetAllNotifications(int userId)
    {
        var notifications = await _notificationService.GetAllMessagesForUser(userId);
        return Ok(_mappper.Map<List<NotificationPostDTO>>(notifications));
    }

    [HttpPost("addForCaller/{message}")]
    public async Task<IActionResult> AddNotificationToCaller([FromBody] NotificationMessage message)
    {
        await _notificationService.AddNotificationToCaller(message.UserId, message.MsgContent);
        return Ok();
    }
    // add notification to all customers
    [HttpPost("addForCustomers/{message}")]
    public async Task<IActionResult> AddNotificationToAllCustomers([FromBody] NotificationMessage message)
    {
        await _notificationService.AddNotificationToAllCustomers(message.MsgContent);
        return Ok();
    }
    // add notification to all admins
    [HttpPost("addForAdmins/{message}")]
    public async Task<IActionResult> AddNotificationToAllAdmins([FromBody] NotificationMessage message)
    {
        await _notificationService.AddNotificationToAllAdmins(message.MsgContent);
        return Ok();
    }

    [HttpPost("AddNotification/{userId}")]
    public async Task<IActionResult> AddNotification(int userId, [FromBody] NotificationPostDTO message)
    {
        await _notificationService.AddNotificationToCaller(userId, message.MsgContent);
        return Ok();
    }

    [HttpPost("MarkAllAsRead/{userId}")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        await _notificationService.MarkAllAsRead(userId);
        return Ok();
    }

    [HttpGet("GetUnreadCount/{userId}")]
    public async Task<IActionResult> GetUnreadCount(int userId)
    {
        var count = await _notificationService.GetUnreadCount(userId);
        return Ok(count);
    }

}
