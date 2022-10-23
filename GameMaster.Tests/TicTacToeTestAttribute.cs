namespace GameMaster.Tests;

public class TicTacToeTestAttribute : GameTestAttribute
{
    public TicTacToeTestAttribute() : base("tic-tac-toe-game-master")
    {
        // Hosted Dev instance (matches dev instance in tasks.json)
        BaseUrl = "https://app-persisted-world-dev.azurewebsites.net";
    }
}
