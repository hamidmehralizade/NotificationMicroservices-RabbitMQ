using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Email.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<Worker> _logger;
        private IModel _channel;

        public Worker(IConnection connection, ILogger<Worker> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = _connection.CreateModel();

            // Setting Exchange and Queue
            _channel.ExchangeDeclare("notifications", ExchangeType.Direct, durable: true);
            _channel.QueueDeclare("email_queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind("email_queue", "notifications", "email");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += ProcessMessage;

            _channel.BasicConsume(queue: "email_queue", autoAck: false, consumer: consumer);

            _logger.LogInformation("Email Worker started and waiting for messages...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessMessage(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var notification = JsonSerializer.Deserialize<NotificationDto>(body);

                await Task.Delay(500); // Sending Email Simulation

                _logger.LogInformation($"Email sent to {notification.Recipient}: {notification.Message}");

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }
    }

    public class NotificationDto
    {
        public string Recipient { get; set; }
        public string Message { get; set; }
    }
}