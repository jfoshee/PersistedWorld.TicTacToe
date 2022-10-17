using static TicTacToe.Console.Constants;
using System.Collections.Immutable;
using TicTacToe.Console.Common;

namespace TicTacToe.Console;

public class TicTacToeClient : ITicTacToeClient
{
    private readonly IGameStateClient gameStateClient;
    private readonly IChangeUserService changeUserService;
    private readonly IGameTestHarness game;
    private readonly IClientNotification clientNotification;

    public TicTacToeClient(IGameStateClient gameStateClient,
                           IChangeUserService changeUserService,
                           IGameTestHarness game,
                           IClientNotification clientNotification)
    {
        this.gameStateClient = gameStateClient ?? throw new ArgumentNullException(nameof(gameStateClient));
        this.changeUserService = changeUserService ?? throw new ArgumentNullException(nameof(changeUserService));
        this.game = game ?? throw new ArgumentNullException(nameof(game));
        this.clientNotification = clientNotification ?? throw new ArgumentNullException(nameof(clientNotification));
    }

    public async Task Start(GameEntityState boardEntity, string opponentId)
    {
        try
        {
            await game.Call.StartGame(boardEntity, opponentId);
        }
        catch (ApiException apiException)
        {
            clientNotification.ShowError(apiException);
        }
    }

    public string GetMessage(GameEntityState boardEntity)
    {
        return game.State(boardEntity).message;
    }

    public bool IsStarted(GameEntityState boardEntity)
    {
        var players = boardEntity.GetPublicValue<IList<object>>(GameMasterId, "players");
        return players?.Count == 2;
    }

    bool IsOwnerOrPlayer(GameEntityState boardEntity)
    {
        var owner = boardEntity.SystemState.OwnerId;
        if (owner == changeUserService.CurrentUserId)
            return true;
        var players = boardEntity.GetPublicValue<IList<object>>(GameMasterId, "players")?.Cast<string>();
        return players is not null && players.Contains(changeUserService.CurrentUserId);
    }

    public bool IsComplete(GameEntityState boardEntity)
    {
        return boardEntity.GetPublicValue<bool>(GameMasterId, "isComplete");
    }

    public async Task Refresh(GameEntityState entity)
    {
        // TODO: Extract refresh function to IGameTestHarness
        // Update entity in place so it is updated in the collection
        var updated = await gameStateClient.GetEntityAsync(entity.Id!) ?? throw new Exception("Failed to fetch updated game board");
        entity.UpdateFrom(updated);
    }

    public async Task<ImmutableList<GameEntityState>> GetBoards()
    {
        // TODO: How is it possible to get boards where I am the opponent, but not in my location?
        // Enumerate the player's boards
        var owned = await gameStateClient.GetEntitiesOwnedAsync()
                    ?? throw new Exception("Failed to fetch owned entities");
        // Get nearby boards where I might be the opponent
        var nearby = await gameStateClient.GetEntitiesNearbyAsync()
                     ?? throw new Exception("Failed to fetch nearby entities");
        var boards = owned.Concat(nearby)
                          .DistinctBy(e => e.Id)
                          .Where(entity => entity.IsTicTacToeBoard())
                          .Where(entity => !IsComplete(entity))
                          .Where(entity => IsOwnerOrPlayer(entity))
                          .ToImmutableList();
        return boards;
    }
}
