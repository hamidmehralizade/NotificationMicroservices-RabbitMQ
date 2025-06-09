using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Notification.Workers;

var builder = Host.CreateApplicationBuilder(args);

// Register RabbitMQ Connection as Singleton
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["RabbitMQ:HostName"] ?? "rabbitmq",
        UserName = builder.Configuration["RabbitMQ:Username"] ?? "admin",
        Password = builder.Configuration["RabbitMQ:Password"] ?? "password",
        DispatchConsumersAsync = true
    };
    return factory.CreateConnection();
});

// Register Workers based on configuration
if (builder.Configuration.GetValue<bool>("Workers:EmailWorker"))
    builder.Services.AddHostedService<EmailWorker>();
if (builder.Configuration.GetValue<bool>("Workers:SmsWorker"))
    builder.Services.AddHostedService<SmsWorker>();
if (builder.Configuration.GetValue<bool>("Workers:PushWorker"))
    builder.Services.AddHostedService<PushWorker>();

var host = builder.Build();
host.Run();