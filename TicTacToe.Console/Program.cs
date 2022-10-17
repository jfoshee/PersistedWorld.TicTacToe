using System.Collections.Immutable;
using TicTacToe.Console.Common;
using static System.Console;
using static TicTacToe.Console.ConsoleUi;
using static TicTacToe.Console.Constants;

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
var player = await gameStateClient.GetUserAsync() ?? throw new Exception("Failed to fetch Player");
// HACK: Move players in old "00" location to new default "00:00:00:00:00"
if (player.SystemState.Location == "00")
{
    await game.Move(player, "00:00:00:00:00");
}
WriteLine($"Hello, {player.DisplayName}. Your location is {player.SystemState.Location}");

while (true)
{
    var choices = new List<(string, Func<Task>)>();
    choices.Add(("New Game", async () =>
    {
        // New game
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();
        WriteLine("New game created");
        await Play(boardEntity);
    }
    ));
    var boards = await GetBoards(gameStateClient);
    foreach (var board in boards)
    {
        choices.Add(($"Resume Game: {board.SystemState.CreatedAt:g} {board.SystemState.Location}", () => Play(board)));
    }
    await Choose(choices);
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
    await Refresh(boardEntity, gameStateClient);
    if (!IsStarted(boardEntity))
    {
        await Start(boardEntity);
    }
    while (!IsComplete(boardEntity))
    {
        PrintBoard(boardEntity, game);
        PrintMessage(boardEntity, game);
        var square = InputOf<int>("Square");
        if (square >= 0 && square <= 8)
        {
            try
            {
                await game.Call.TakeTurn(boardEntity, square);
            }
            catch (ApiException apiException)
            {
                WriteLine(apiException.SimpleMessage());
            }
        }
        else
            await Refresh(boardEntity, gameStateClient);
    }
    PrintBoard(boardEntity, game);
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

static void PrintBoard(GameEntityState boardEntity, IGameTestHarness game)
{
    var b = game.State(boardEntity).board;
    WriteLine($" {b[0]} | {b[1]} | {b[2]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[3]} | {b[4]} | {b[5]} ");
    WriteLine("---+---+---");
    WriteLine($" {b[6]} | {b[7]} | {b[8]} ");
}

static bool IsStarted(GameEntityState boardEntity)
{
    var players = boardEntity.GetPublicValue<IList<object>>(GameMasterId, "players");
    return players?.Count == 2;
}

static bool IsOwnerOrPlayer(GameEntityState boardEntity, IChangeUserService changeUserService)
{
    var owner = boardEntity.SystemState.OwnerId;
    if (owner == changeUserService.CurrentUserId)
        return true;
    var players = boardEntity.GetPublicValue<IList<object>>(GameMasterId, "players")?.Cast<string>();
    return players is not null && players.Contains(changeUserService.CurrentUserId);
}

static bool IsComplete(GameEntityState boardEntity)
{
    return boardEntity.GetPublicValue<bool>(GameMasterId, "isComplete");
}

static async Task Refresh(GameEntityState entity, IGameStateClient gameStateClient)
{
    // TODO: Extract refresh function to IGameTestHarness
    // Update entity in place so it is updated in the collection
    var updated = await gameStateClient.GetEntityAsync(entity.Id!) ?? throw new Exception("Failed to fetch updated game board");
    entity.UpdateFrom(updated);
}

async Task<ImmutableList<GameEntityState>> GetBoards(IGameStateClient gameStateClient)
{
    var nearby = await gameStateClient.GetEntitiesNearbyAsync()
                 ?? throw new Exception("Failed to fetch nearby entities");
    // Enumerate the player's boards
    var owned = await gameStateClient.GetEntitiesOwnedAsync()
                ?? throw new Exception("Failed to fetch owned entities");
    // TODO: Active (not complete) boards only
    var boards = owned.Concat(nearby)
                      .DistinctBy(e => e.Id)
                      .Where(entity => entity.IsTicTacToeBoard())
                      .Where(entity => IsOwnerOrPlayer(entity, changeUserService))
                      .ToImmutableList();
    return boards;
}
