using Carter;
using Notification.API.Enums;
using Notification.API.Models;
using Notification.API.Services;

namespace Notification.API.Modules
{
    public record NotificationRequest(NotificationType Type, string Recipient, string Message);

    public class NotificationModule : ICarterModule
    {
        private readonly INotificationService _notificationService;

        public NotificationModule(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/notifications");

            group.MapPost("/", async (HttpContext ctx, NotificationRequest request) =>
            {
                await _notificationService.PublishNotificationAsync(new NotificationDto
                {
                    Type = request.Type,
                    Recipient = request.Recipient,
                    Message = request.Message
                });

                return Results.Accepted();
            })
            .WithName("SendNotification")
            .Produces(202);
        }
    }
}
