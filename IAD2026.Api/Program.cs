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
// Infrastructure
builder.Services.AddInfrastructure();

// MediatR + Validation pipeline
//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly);
//    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
//});

//builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskCommandValidator>();

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.Console()
      .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
app.Run();

// REQUIRED for integration tests
public partial class Program { }