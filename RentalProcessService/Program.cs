using Common.Models;
using Common.Models.Events;
using Common.Services;
using MassTransit;
using RentalProcessService.Saga;
using RentalProcessService.Services;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServiceDiscovery(opt => opt.UseEureka());

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
            r.Connection = "mongodb://adminuser:password123@192.168.1.199:31000";
            r.DatabaseName = "sagas";
            r.CollectionName = "sagas";
        });
});

builder.Services.AddScoped<IProducer<IRentalSubmitted>, Producer<IRentalSubmitted>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();