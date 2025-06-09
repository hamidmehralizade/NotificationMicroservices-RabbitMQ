using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Notification.Workers
{
    public class EmailWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<EmailWorker> _logger;
        private IModel _channel;

        public EmailWorker(IConnection connection, ILogger<EmailWorker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Create channel and set up RabbitMQ topology
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("notifications", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("email_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("email_queue", "notifications", "email");

            // Configure consumer
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var notification = JsonSerializer.Deserialize<NotificationDto>(body);

                    // Simulate email processing
                    await Task.Delay(500);
                    _logger.LogInformation($"Email sent to {notification.Recipient}: {notification.Message}");

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "email_queue", autoAck: false, consumer: consumer);
            _logger.LogInformation("Email Worker started listening for messages...");

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