using GameClient;

namespace GameMaster.Tests;

public class TicTacToeTestAttribute : GameTestAttribute
{
    public TicTacToeTestAttribute() : base(Constants.GameMasterId)
    {
        // Hosted Dev instance (matches dev instance in tasks.json)
        BaseUrl = "https://app-persisted-world-dev.azurewebsites.net";
    }
}
