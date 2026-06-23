using IAD2026.Api.Middlewares;
using IAD2026.Application;
using IAD2026.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.Console()
      .WriteTo.File("logs/log.txt",
          rollingInterval: RollingInterval.Day);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }