using DemonstrateRebusTimeoutIssueOnApi.Handlers;
using DemonstrateRebusTimeoutIssueOnApi.Models;
using Rebus.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRebus(
        c => c.Transport(c => c.UseRabbitMq("amqp://guest:guest@localhost", "demonstrate-rebus-timeout-issue-api")),
        onCreated: b => b.Subscribe<TestApiEvent>())
    .AddRebusHandler<TestApiEventHandler>();

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

public partial class Program
{
}