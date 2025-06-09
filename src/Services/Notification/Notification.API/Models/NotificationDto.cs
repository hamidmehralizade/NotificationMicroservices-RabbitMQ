using Notification.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace Notification.API.Models;

public class NotificationDto
{
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    [Required]
    public NotificationType Type { get; set; } // enum: Email, Sms, Push

    [Required]
    public string Recipient { get; set; } // Email/Phone Number/DeviceToken

    [Required]
    public string Message { get; set; }

    public string Subject { get; set; } // For Email

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // For Tracking the Status
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    // Custom Metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}
