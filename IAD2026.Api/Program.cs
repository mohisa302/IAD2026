using Carter;
using IAD2026.Api.Middlewares;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using IAD2026.Api.Middlewares;
using IAD2026.Application;
using IAD2026.Infrastructure;

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
      .MinimumLevel.Warning()                    // ← Only Warnings + Errors
      .WriteTo.Console()
      .WriteTo.File("logs/log-.txt",
          rollingInterval: RollingInterval.Day,
          retainedFileCountLimit: 7);
});



var app = builder.Build();

// === Middleware Pipeline ===
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();

// ====================== ADD THIS ======================
app.UseCors("AllowSwagger");
// ======================================================
//}

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