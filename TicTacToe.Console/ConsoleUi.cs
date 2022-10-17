using static System.Console;

namespace TicTacToe.Console;

public static class ConsoleUi
{
    public static string Input(string prompt)
    {
        Write($"{prompt}> ");
        var input = ReadLine() ?? "";
        return input;
    }

    //static T InputOf<T>(string prompt)
    //{
    //    var text = Input(prompt);
    //    var type = typeof(T);
    //    return (T)Convert.ChangeType(text, type);
    //}
}
