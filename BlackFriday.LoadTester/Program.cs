// See https://aka.ms/new-console-template for more information
using BlackFriday.LoadTester.BackgroundServices.ScenarioExecution;
using BlackFriday.LoadTester.BackgroundServices.ScenarioExecution.Abstraction;
using BlackFriday.LoadTester.BackgroundServices.ScenarioExecution.LoadTestScenarios;
using BlackFriday.LoadTester.UseCases;
using BlackFriday.LoadTester.UseCases.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("black_friday", options => { options.BaseAddress = new Uri("http://localhost:32768"); });

builder.Services.AddHostedService<ScenarioExecutorBackgroundService>();

builder.Services.AddSingleton<IDiceRoller, DiceRoller>();
builder.Services.AddSingleton<ILoadTestScenarioProvider, LoadTestScenarioProvider>();

builder.Services.AddSingleton<ILoadTestScenario, GetCategoriesScenario>();
builder.Services.AddSingleton<ILoadTestScenario, BrowseCategoriesScenario>();
builder.Services.AddSingleton<ILoadTestScenario, BrowseItemsScenario>();
builder.Services.AddSingleton<ILoadTestScenario, FillBasketScenario>();
builder.Services.AddSingleton<ILoadTestScenario, CheckoutBasketScenario>();

builder.Services.AddSingleton<ICategoriesGetter, CategoriesGetter>();
builder.Services.AddSingleton<ICategoriesItemsGetter, CategoriesItemsGetter>();
builder.Services.AddSingleton<IItemsGetter, ItemsGetter>();
builder.Services.AddSingleton<IBasketFiller, BasketFiller>();
builder.Services.AddSingleton<IBasketCheckouter, BasketCheckouter>();

var app = builder.Build();
app.Run();
