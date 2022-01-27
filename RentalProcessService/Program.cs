using Common;
using Common.Events;
using Common.Models;
using MassTransit;
using RentalProcessorService.Saga;
using RentalProcessorService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfiguration =
            builder.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

        cfg.Host(new Uri(rabbitMqConfiguration.Host), h =>
        {
            h.Username(rabbitMqConfiguration.Username);
            h.Password(rabbitMqConfiguration.Password);
        });

        cfg.ReceiveEndpoint("rental", e => { e.ConfigureSaga<RentalState>(context); });

        cfg.ConfigureEndpoints(context);
    });

    x.AddSagaStateMachine<RentalStateMachine, RentalState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://adminuser:password123@192.168.1.200:32000";
            r.DatabaseName = "sagas";
            r.CollectionName = "sagas";
        });
});

builder.Services.AddScoped<IProducer<IRentalSubmitted>, Producer<IRentalSubmitted>>();

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();