using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Infrastructure.BackgroundServices;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);


builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
    options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Configuration["ServiceName"]!));
    options.AddOtlpExporter();
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("foodDb")));
builder.Services.AddScoped<IFoodReservationsDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Transient);

builder.Services.AddHostedService<DatabaseUpdater>();

builder.Services
    .AddOpenTelemetry()
    .WithMetrics(options => options.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter())
    .WithTracing(options => options.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Configuration["ServiceName"]!))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter());

builder.Services.AddDateOnlyTimeOnlyStringConverters();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());

var app = builder.Build();

app.Logger.LogInformation(app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); // allow credentials

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.UseAuthorization();

app.MapControllers();

app.Run();