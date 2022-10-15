namespace GameMaster.Tests;

public class TicTacToeBoardTest
{
    [Theory(DisplayName = "Default"), TicTacToeTest]
    public async Task DefaultBoard(IGameTestHarness game)
    {
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();

        boardEntity.DisplayName.Should().Be("TicTacToe Board");
        var boardState = game.State(boardEntity);
        Assert.Equal("TicTacToeBoard", boardState.type);
        Assert.Equivalent(Enumerable.Repeat(" ", 9), boardState.board);
        Assert.False(boardState.isComplete);
        Assert.Equal("", boardState.message);
    }
}
