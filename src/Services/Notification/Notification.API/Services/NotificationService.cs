using Polly;
using RabbitMQ.Client;
using System.Text.Json;
using Notification.API.Models;

namespace Notification.API.Services;

public interface INotificationService
{
    Task PublishNotificationAsync(NotificationDto notification);
}

public class RabbitMqNotificationService : INotificationService
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqNotificationService> _logger;

    public RabbitMqNotificationService(IConnection connection, ILogger<RabbitMqNotificationService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishNotificationAsync(NotificationDto notification)
    {
        var policy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(async () =>
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare("notifications", ExchangeType.Direct, durable: true);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var body = JsonSerializer.SerializeToUtf8Bytes(notification);

            channel.BasicPublish(
                exchange: "notifications",
                routingKey: notification.Type.ToString().ToLower(),
                basicProperties: properties,
                body: body);

            _logger.LogInformation($"Published notification: {notification.Type}");
        });
    }
}
