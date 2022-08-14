using DockService;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.SetupServices(builder.Configuration);
builder.Services.AddServiceDiscovery(opt => opt.UseEureka());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.Map<HealthEndpoint>();
    endpoints.Map<InfoEndpoint>();
});

app.Run();