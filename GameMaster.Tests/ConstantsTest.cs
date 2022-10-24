using GameClient;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GameMaster.Tests;

public class ConstantsTest
{
    [Fact(DisplayName = "Matching GameMaster ID")]
    public void ConstantsMatch()
    {
        // Verifies that the GameMaster ID in Constants matches configuration in gamemaster.json
        // Current test directory will be in the output path
        // e.g. D:\Source\PersistedWorld.TicTacToe\GameMaster.Tests\bin\Debug\net6.0
        var directory = Directory.GetCurrentDirectory();
        // Walk up to the repo root
        for (int i = 0; i < 4; i++)
        {
            directory = Directory.GetParent(directory)!.FullName;
        }
        // Ensure we are at the repo root
        // e.g. D:\Source\PersistedWorld.TicTacToe\
        Directory.EnumerateDirectories(directory)
                 .Select(Path.GetFileName)
                 .Should()
                 .Contain(".git");
        var jsonPath = Path.Combine(directory, "GameMaster", "gamemaster.json");
        var json = File.ReadAllText(jsonPath);
        var gameMaster = JsonSerializer.Deserialize<JsonObject>(json) ?? throw new Exception("Failed to deserialize gamemaster.json");
        Assert.Equal(Constants.GameMasterId, (string?)gameMaster["GameMasterEntityId"]);
    }
}
