using BlackFriday.LoadTester.BackgroundServices.ScenarioExecution.Abstraction;
using Microsoft.Extensions.Hosting;

namespace BlackFriday.LoadTester.BackgroundServices.ScenarioExecution;

internal class ScenarioExecutorBackgroundService : BackgroundService
{
	private readonly IDiceRoller _diceRoller;
	private readonly ILoadTestScenarioProvider _scenarioFactory;

	public ScenarioExecutorBackgroundService(ILoadTestScenarioProvider scenarioFactory, IDiceRoller diceRoller)
	{
		_scenarioFactory = scenarioFactory ?? throw new ArgumentNullException(nameof(scenarioFactory));
		_diceRoller = diceRoller ?? throw new ArgumentNullException(nameof(diceRoller));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var users = new List<Task>();
		foreach (var _ in Enumerable.Range(0, 5))
		{
			users.Add(Task.Run(async () =>
			{
				var scenarioType = _diceRoller.RollTheDice();
				var scenario = _scenarioFactory.MakeScenario(scenarioType);
				await scenario.ExecuteAsync(stoppingToken);
			}, stoppingToken));
		}
		await Task.WhenAll(users);
	}
}
