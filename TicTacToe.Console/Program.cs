using System.Collections.Immutable;
using static System.Console;
using static TicTacToe.Console.Constants;
using static TicTacToe.Console.ConsoleUi;
using TicTacToe.Console.Common;

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
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();
        WriteLine("New game created");
        await Start(boardEntity);
        await Play(boardEntity);
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
        choices.Add(($"Resume Game: {board.SystemState.CreatedAt:g}", () => Play(board)));
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

async Task Start(GameEntityState boardEntity)
{
    var opponentId = Input("Opponent ID");
    try
    {
        await game.Call.StartGame(boardEntity, opponentId);
    }
    catch (ApiException apiException)
    {
        WriteLine(apiException.SimpleMessage());
    }
}

async Task Play(GameEntityState boardEntity)
{
    // TODO: If not started, Start
    while (!boardEntity.GetPublicValue<bool>(GameMasterId, "isComplete"))
    {
        PrintMessage(boardEntity, game);
        var b = game.State(boardEntity).board;
        WriteLine($" {b[0]} | {b[1]} | {b[2]} ");
        WriteLine("---+---+---");
        WriteLine($" {b[3]} | {b[4]} | {b[5]} ");
        WriteLine("---+---+---");
        WriteLine($" {b[6]} | {b[7]} | {b[8]} ");
        await Task.Yield();
        var square = InputOf<int>("Square");
        try
        {
            await game.Call.TakeTurn(boardEntity, square);
        }
        catch (ApiException apiException)
        {
            WriteLine(apiException.SimpleMessage());
        }
    }
    PrintMessage(boardEntity, game);
}

static string GetMessage(GameEntityState boardEntity, IGameTestHarness game)
{
    return game.State(boardEntity).message;
}

static void PrintMessage(GameEntityState boardEntity, IGameTestHarness game)
{
    var message = GetMessage(boardEntity, game);
    WriteLine(message);
}
