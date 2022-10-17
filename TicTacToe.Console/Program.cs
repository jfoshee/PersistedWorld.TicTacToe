using TicTacToe.Console.Common;
using static System.Console;
using static TicTacToe.Console.ConsoleUi;
using static TicTacToe.Console.Constants;

// Setup services
var baseUrl = "https://localhost:7264";
var services = new ServiceCollection();
services.AddGameStateClientServices(GameMasterId, baseUrl);
services.AddTransient<ITicTacToeClient, TicTacToeClient>();
services.AddTransient<IClientNotification, ConsoleClientNotification>();

var serviceProvider = services.BuildServiceProvider();
var game = serviceProvider.GetRequiredService<IGameTestHarness>();
var gameStateClient = serviceProvider.GetRequiredService<IGameStateClient>();
var changeUserService = serviceProvider.GetRequiredService<IChangeUserService>();
var ticTacToeClient = serviceProvider.GetRequiredService<ITicTacToeClient>();
var clientNotification = serviceProvider.GetRequiredService<IClientNotification>();

// Login
var playerId = Input("Player ID");
playerId = playerId.Trim();
await changeUserService.ChangeUserAsync(playerId);
var player = await gameStateClient.GetUserAsync() ?? throw new Exception("Failed to fetch Player");
// HACK: Move players in old "00" location to new default "00:00:00:00:00"
if (player.SystemState.Location == "00")
{
    await game.Move(player, "00:00:00:00:00");
}
WriteLine($"Hello, {player.DisplayName}. Your location is {player.SystemState.Location}");

bool quit = false;
while (!quit)
{
    var choices = new List<(string, Func<Task>)>();
    choices.Add(("Quit", () =>
    {
        quit = true;
        return Task.CompletedTask;
    }
    ));
    choices.Add(("Refresh", () => Task.CompletedTask));
    choices.Add(("New Game", async () => await NewGame(game, gameStateClient)));
    var boards = await ticTacToeClient.GetBoards();
    foreach (var board in boards)
    {
        choices.Add(($"Resume Game: {board.SystemState.CreatedAt.LocalDateTime:g} {board.SystemState.Location}", () => Play(board)));
    }
    await Choose(choices);
}


async Task NewGame(IGameTestHarness game, IGameStateClient? gameStateClient)
{
    GameEntityState boardEntity = await game.Create.TicTacToeBoard();
    WriteLine("New game created");
    await Play(boardEntity);
}

async Task Play(GameEntityState boardEntity)
{
    await ticTacToeClient.Refresh(boardEntity);
    if (!ticTacToeClient.IsStarted(boardEntity))
    {
        var opponentId = Input("Opponent ID");
        await ticTacToeClient.Start(boardEntity, opponentId);
    }
    while (!ticTacToeClient.IsComplete(boardEntity))
    {
        // HACK: Refresh here because e.g. might have tried to take turn in occupied square
        await ticTacToeClient.Refresh(boardEntity);
        PrintBoard(boardEntity, game);
        PrintMessage(boardEntity);
        var square = InputOf<int>("Square Index (Negative number to leave game)");
        if (square >= 0 && square <= 8)
        {
            try
            {
                await game.Call.TakeTurn(boardEntity, square);
            }
            catch (ApiException apiException)
            {
                clientNotification.ShowError(apiException);
            }
        }
        else if (square < 0)
            // Enter negative number to leave this board
            return;
    }
    PrintBoard(boardEntity, game);
    PrintMessage(boardEntity);
}

void PrintMessage(GameEntityState boardEntity)
{
    var message = ticTacToeClient.GetMessage(boardEntity);
    WriteLine(message);
}

static void PrintBoard(GameEntityState boardEntity, IGameTestHarness game)
{
    var b = game.State(boardEntity).board;
    WriteLine($" {b[0]} | {b[1]} | {b[2]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[3]} | {b[4]} | {b[5]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[6]} | {b[7]} | {b[8]} ");
}
