﻿//// See https://aka.ms/new-console-template for more information
//using BlackFriday.Infrastructure.Persistence;
//using BlackFriday.LoadTester.BackgroundServices;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql("Host=localhost;Database=food_db;Username=postgres;Password=!@#123qwe"));
//builder.Services.AddHttpClient("food_reservation", options => { options.BaseAddress = new Uri("http://localhost:32768"); });

//builder.Services.AddHostedService<DailyFoodsGetter>();
//builder.Services.AddHostedService<UsersFoodsGetter>();
//builder.Services.AddHostedService<UsersFoodsDeliverer>();
//builder.Services.AddHostedService<FoodIncreaser>();
//builder.Services.AddHostedService<FoodReservor>();

//var app = builder.Build();
//app.Run();

Console.WriteLine("Hello World");