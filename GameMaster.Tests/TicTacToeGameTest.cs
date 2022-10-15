namespace GameMaster.Tests;

public class TicTacToeGameTest
{
    [Theory(DisplayName = "Start Game"), TicTacToeTest]
    public async Task StartGame(IGameTestHarness game)
    {
        // TODO: Set player display names
        var player2 = await game.NewCurrentPlayer();
        var player1 = await game.NewCurrentPlayer();
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();

        await game.Call.StartGame(boardEntity, player2.Id);

        var boardState = game.State(boardEntity);
        Assert.Equivalent(Enumerable.Repeat(" ", 9), boardState.board);
        Assert.False(boardState.isComplete);
        Assert.Equivalent(new[] { player1.Id, player2.Id }, boardState.players);
        Assert.Equal("The TicTacToe game has started. It is Player's turn!", boardState.message);
    }
}
