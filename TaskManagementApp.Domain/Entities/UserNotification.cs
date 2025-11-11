//Should Implement Notifications feature later

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApp.Domain
{
    public class UserNotification
    {
        public long NotificationID { get; set; }

        public long UserID { get; set; }

        [Required]
        public AlertType Type { get; set; }

        [Required]
        [StringLength(500)]
        public string Target { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
