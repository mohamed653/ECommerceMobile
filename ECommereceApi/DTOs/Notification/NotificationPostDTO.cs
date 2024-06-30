using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs.Notification
{
    public class NotificationPostDTO
    {

        public int UserId { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public string MsgContent { get; set; }

        public DateTime SendingDate { get; set; }

        [StringLength(100)]
        public string HiddenLink { get; set; }

    }
}
