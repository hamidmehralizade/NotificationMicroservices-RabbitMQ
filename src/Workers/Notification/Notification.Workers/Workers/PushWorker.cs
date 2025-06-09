using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Notification.Workers
{
    public class PushWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<PushWorker> _logger;
        private IModel _channel;

        public PushWorker(IConnection connection, ILogger<PushWorker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("notifications", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("push_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("push_queue", "notifications", "push");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var notification = JsonSerializer.Deserialize<NotificationDto>(body);

                    // Simulate push notification processing (fastest)
                    await Task.Delay(200);
                    _logger.LogInformation($"Push sent to device {notification.Recipient}: {notification.Message}");

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing push notification");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "push_queue", autoAck: false, consumer: consumer);
            _logger.LogInformation("Push Worker started listening for messages...");

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }
}