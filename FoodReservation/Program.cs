using FoodReservation.Application.Repositories.Abstractions;
using FoodReservation.Infrastructure.BackgroundServices;
using FoodReservation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("foodDb")));
builder.Services.AddScoped<IFoodReservationsDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Transient);

builder.Services.AddHostedService<DatabaseUpdater>();

builder.Services.AddDateOnlyTimeOnlyStringConverters();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();