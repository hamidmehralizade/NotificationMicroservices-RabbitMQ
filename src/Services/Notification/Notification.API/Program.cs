using Carter;
using Notification.API.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Notification API", Version = "v1" });
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");

    var factory = new ConnectionFactory()
    {
        HostName = rabbitConfig["HostName"] ?? "rabbitmq",
        UserName = rabbitConfig["Username"] ?? "admin",
        Password = rabbitConfig["Password"] ?? "password",
        DispatchConsumersAsync = true
    };
    return factory.CreateConnection();
});

builder.Services.AddSingleton<INotificationService, RabbitMqNotificationService>();

builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();