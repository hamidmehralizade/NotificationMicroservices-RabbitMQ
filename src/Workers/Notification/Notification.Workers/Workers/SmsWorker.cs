using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Notification.Workers
{
    public class SmsWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<SmsWorker> _logger;
        private IModel _channel;

        public SmsWorker(IConnection connection, ILogger<SmsWorker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("notifications", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("sms_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("sms_queue", "notifications", "sms");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var notification = JsonSerializer.Deserialize<NotificationDto>(body);

                    // Simulate SMS processing (faster than email)
                    await Task.Delay(300);
                    _logger.LogInformation($"SMS sent to {notification.Recipient}: {notification.Message}");

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing SMS message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "sms_queue", autoAck: false, consumer: consumer);
            _logger.LogInformation("SMS Worker started listening for messages...");

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