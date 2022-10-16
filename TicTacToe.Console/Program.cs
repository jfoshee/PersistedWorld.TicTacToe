using System.Collections.Immutable;
using static TicTacToe.Console.Constants;

// Setup services
var baseUrl = "https://localhost:7264";
var services = new ServiceCollection();
services.AddGameStateClientServices(GameMasterId, baseUrl);
var serviceProvider = services.BuildServiceProvider();
var game = serviceProvider.GetService<IGameTestHarness>();
var gameStateClient = serviceProvider.GetRequiredService<IGameStateClient>();
var changeUserService = serviceProvider.GetRequiredService<IChangeUserService>();

// Startup Terminal.Gui
Application.Init();

// Login
var playerId = InputBox.Show("Player ID", "Login", null);
// HACK: Blocking calls because async versions cause a deadlock w/ Terminal.Gui
changeUserService.ChangeUser(playerId);
var player = gameStateClient.GetUser();

MessageBox.Query("Greet", $"Hello, {player.DisplayName}!", "OK");

// Terminate the Terminal.Gui
Application.Shutdown();

// Enumerate the player's boards
var owned = await gameStateClient.GetEntitiesOwnedAsync()
            ?? throw new Exception("Failed to fetch owned entities");
var boards = owned.Where(entity => entity.IsTicTacToeBoard())
                  .ToImmutableList();
Console.WriteLine("Your game boards: ");
for (int i = 0; i < boards.Count; i++)
{
    var board = boards[i];
    Console.WriteLine($"{i} {board.Id}");
}
