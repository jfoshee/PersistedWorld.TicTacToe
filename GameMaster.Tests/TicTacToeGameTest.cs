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

    [Theory(DisplayName = "Take Turns"), TicTacToeTest]
    public async Task TakeTurns(IGameTestHarness game)
    {
        // TODO: Set player display names
        var player2 = await game.NewCurrentPlayer();
        var player1 = await game.NewCurrentPlayer();
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();

        await game.Call.StartGame(boardEntity, player2.Id);
        await game.Call.TakeTurn(boardEntity, 3);

        var boardState = game.State(boardEntity);
        Assert.Equivalent(new [] { " ", " ", " ", "x", " ", " ", " ", " ", " " }, boardState.board);
        Assert.False(boardState.isComplete);
        // TODO: Assert.Equal("It is Player2's turn!", boardState.message);

        game.SetCurrentPlayer(player2);
        await game.Call.TakeTurn(boardEntity, 4);
        boardState = game.State(boardEntity);
        Assert.Equivalent(new [] { " ", " ", " ", "x", "o", " ", " ", " ", " " }, boardState.board);
    }

    [Theory(DisplayName = "Play Game to Win"), TicTacToeTest]
    public async Task PlayToWin(IGameTestHarness game)
    {
        var player2 = await game.NewCurrentPlayer();
        var player1 = await game.NewCurrentPlayer();
        GameEntityState boardEntity = await game.Create.TicTacToeBoard();

        await game.Call.StartGame(boardEntity, player2.Id);
        await game.Call.TakeTurn(boardEntity, 3);

        game.SetCurrentPlayer(player2);
        await game.Call.TakeTurn(boardEntity, 2);

        game.SetCurrentPlayer(player1);
        await game.Call.TakeTurn(boardEntity, 4);

        game.SetCurrentPlayer(player2);
        await game.Call.TakeTurn(boardEntity, 6);

        game.SetCurrentPlayer(player1);
        await game.Call.TakeTurn(boardEntity, 5);

        var boardState = game.State(boardEntity);
        Assert.Equivalent(new [] { " ", " ", "o", "x", "x", "x", "o", " ", " " }, boardState.board);
        Assert.True(boardState.isComplete);
        Assert.Equal(player1.Id, boardState.winner);
        // TODO: message == "Player1 wins!"
    }
}
