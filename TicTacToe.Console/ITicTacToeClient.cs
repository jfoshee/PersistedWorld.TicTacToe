using System.Collections.Immutable;

namespace TicTacToe.Console;

public interface ITicTacToeClient
{
    Task<ImmutableList<GameEntityState>> GetBoards();
    string GetMessage(GameEntityState boardEntity);
    bool IsComplete(GameEntityState boardEntity);
    bool IsStarted(GameEntityState boardEntity);
    Task Refresh(GameEntityState entity);
    Task Start(GameEntityState boardEntity, string opponentId);
}
