using System.Collections.Immutable;
using static System.Console;
using static TicTacToe.Console.Constants;
using static TicTacToe.Console.ConsoleUi;

// Setup services
var baseUrl = "https://localhost:7264";
var services = new ServiceCollection();
services.AddGameStateClientServices(GameMasterId, baseUrl);
var serviceProvider = services.BuildServiceProvider();
var game = serviceProvider.GetRequiredService<IGameTestHarness>();
var gameStateClient = serviceProvider.GetRequiredService<IGameStateClient>();
var changeUserService = serviceProvider.GetRequiredService<IChangeUserService>();

// Login
var playerId = Input("Player ID");
playerId = playerId.Trim();
await changeUserService.ChangeUserAsync(playerId);

while (true)
{
    var choices = new List<(string, Func<Task>)>();
    choices.Add(("New Game", async () =>
    {
        // New game
        GameEntityState boardState = await game.Create.TicTacToeBoard();
        WriteLine("New game created");
    }
    ));
    // Enumerate the player's boards
    var owned = await gameStateClient.GetEntitiesOwnedAsync()
                ?? throw new Exception("Failed to fetch owned entities");
    // TODO: Active (not complete) boards only
    var boards = owned.Where(entity => entity.IsTicTacToeBoard())
                      .ToImmutableList();
    foreach (var board in boards)
    {
        choices.Add(($"Resume Game: {board.SystemState.CreatedAt.ToString("g")}", () => Task.Delay(0)));
    }

    for (int i = 0; i < choices.Count; i++)
    {
        var (text, action) = choices[i];
        WriteLine($"{i} {text}");
    }
    var choice = InputOf<int>("Choose");
    var chosenAction = choices[choice].Item2;
    await chosenAction();
}
