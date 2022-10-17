namespace TicTacToe.Console.Common;

public static class EntityExtensions
{
    public static bool IsTicTacToeBoard(this GameEntityState entity)
    {
        var type = entity.GetPublicValue<string>(Constants.GameMasterId, "type");
        return type == "TicTacToeBoard";
    }
}
