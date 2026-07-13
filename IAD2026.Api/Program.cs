using Carter;
using Hangfire;
using IAD2026.Api.Middlewares;
using IAD2026.Application;
using IAD2026.Application.Options;
using IAD2026.BackgroundJobs.Jobs;
using IAD2026.BackgroundJobss.Options;
using IAD2026.Domain.Entities;
using IAD2026.Infrastructure;
using IAD2026.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// === Services ===
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
});

// Add Carter
builder.Services.AddCarter();

// ====================== ADD THIS ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ======================================================

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .MinimumLevel.Information()                                  // 1. Set global minimum back to Information
      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)     // 2. Mute noisy ASP.NET Core framework logs
      .MinimumLevel.Override("System", LogEventLevel.Warning)        // 3. Mute system-level routing logs
      .MinimumLevel.Override("Hangfire", LogEventLevel.Information)  // 4. (Optional) Explicitly ensure Hangfire is visible
      .WriteTo.Console()
      .WriteTo.File("logs/log-.txt",
          rollingInterval: RollingInterval.Day,
          retainedFileCountLimit: 7);
});

// Register strongly-typed options
builder.Services.Configure<ExternalApiOptions>(
    builder.Configuration.GetSection("ExternalSystems"));

var app = builder.Build();
// ====================== SEED DUMMY DATA ======================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Insert fake tasks if the queue is empty
    if (!db.TaskQueue.Any())
    {
        db.TaskQueue.AddRange(
            new OutboxTask
            {
                TaskType = "SmsNotification",
                ReferenceId = Guid.NewGuid().ToString(),
                Status = OutboxTaskStatus.Pending,
                Payload = "Test SMS Payload"
            },
            new OutboxTask
            {
                TaskType = "DataScrubbing",
                ReferenceId = Guid.NewGuid().ToString(),
                Status = OutboxTaskStatus.Pending,
                Payload = "Test Scrubbing Payload"
            }
        );
        db.SaveChanges();
    }
}
// === Middleware Pipeline ===
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();

// ====================== ADD THIS ======================
app.UseCors("AllowSwagger");
// ====================== HANGFIRE DASHBOARD & SCHEDULING ======================

// 1. Enable the Hangfire Dashboard UI
app.UseHangfireDashboard("/hangfire");

// 2. Retrieve the dynamically configured Cron settings
var hangfireSettings = app.Services.GetRequiredService<IOptions<HangfireSettings>>().Value;

// 3. Register the recurring job that polls your database outbox
RecurringJob.AddOrUpdate<OutboxProcessorJob>(
    "outbox-distributor-job",
    job => job.DistributePendingTasksAsync(CancellationToken.None),
    hangfireSettings.SmsQueueProcessorCron);

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (ctx, elapsed, ex) =>
        ex != null || ctx.Response.StatusCode >= 400 ? LogEventLevel.Error : LogEventLevel.Debug;
});
app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

// === Carter ===
app.MapCarter();

app.Run();

public partial class Program { }