using Email.Worker;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Register Connection RabbitMQ as Singleton
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "rabbitmq",
        UserName = "admin",
        Password = "memo123456",
        DispatchConsumersAsync = true
    };
    return factory.CreateConnection();
});

// Register Worker as Service Background
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();