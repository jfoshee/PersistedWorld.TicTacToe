using static System.Console;
using static TicTacToe.Console.ConsoleUi;
using static GameClient.Constants;

// Setup services
var baseUrl = "https://localhost:7264";
var services = new ServiceCollection();
services.AddGameStateClientServices(GameMasterId, baseUrl);
services.AddTransient<ITicTacToeClient, TicTacToeClient>();
services.AddTransient<IClientNotification, ConsoleClientNotification>();
var serviceProvider = services.BuildServiceProvider();
var ticTacToeClient = serviceProvider.GetRequiredService<ITicTacToeClient>();

// Login
bool authenticated = false;
while (!authenticated)
{
    var playerId = Input("Player ID");
    authenticated = await ticTacToeClient.Login(playerId);
}

// Game Loop
bool quit = false;
while (!quit)
{
    var choices = new List<(string, Func<Task>)>
    {
        ("Quit", () => { quit = true; return Task.CompletedTask; }),
        ("Refresh", () => Task.CompletedTask),
        ("New Game", () => NewGame())
    };
    var boards = await ticTacToeClient.GetBoards();
    foreach (var board in boards)
    {
        choices.Add(($"Resume Game: {board.SystemState.CreatedAt.LocalDateTime:g} {board.SystemState.Location}", () => Play(board)));
    }
    await Choose(choices);
}


async Task NewGame()
{
    var boardEntity = await ticTacToeClient.CreateNewGame();
    if (boardEntity is not null)
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
        PrintBoard(boardEntity);
        PrintMessage(boardEntity);
        var square = InputOf<int>("Square Index (Negative number to leave game)");
        if (square >= 0 && square <= 8)
        {
            await ticTacToeClient.TakeTurn(boardEntity, square);
        }
        else if (square < 0)
            // Enter negative number to leave this board
            return;
    }
    PrintBoard(boardEntity);
    PrintMessage(boardEntity);
}

void PrintMessage(GameEntityState boardEntity)
{
    var message = ticTacToeClient.GetMessage(boardEntity);
    WriteLine(message);
}

void PrintBoard(GameEntityState boardEntity)
{
    var b = ticTacToeClient.GetBoardState(boardEntity);
    WriteLine($" {b[0]} | {b[1]} | {b[2]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[3]} | {b[4]} | {b[5]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[6]} | {b[7]} | {b[8]} ");
}
