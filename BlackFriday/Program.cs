using BlackFriday.Application.Persistence.Abstraction;
using BlackFriday.Infrastructure.BackgroundServices;
using BlackFriday.Infrastructure.Persistence;
using BlackFriday.Infrastructure.Persistence.DbContextFactory;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging => logging.AddOpenTelemetry(options =>
{
	options.IncludeFormattedMessage = true;
	options.IncludeScopes = true;
	options.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddService(builder.Configuration["ServiceName"]!));
	options.AddOtlpExporter();
}));

// Add services to the container.

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("BlackFridayDb")));
builder.Services.AddSingleton<IBlackFridayDbContextFactory, BlackFridayDbContextFactory>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediator();

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
		.AddNpgsql()
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
