namespace Notification.Workers;

public class NotificationDto
{
    public string Recipient { get; set; } // Email/Phone/DeviceToken
    public string Message { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
