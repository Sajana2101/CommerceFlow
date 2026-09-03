using CommerceFlow.NotificationService.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(
        RabbitMqOptions.SectionName));

builder.Services.AddHostedService<
    PaymentNotificationWorker>();

var host = builder.Build();

host.Run();