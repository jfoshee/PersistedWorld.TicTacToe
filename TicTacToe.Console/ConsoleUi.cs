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

    public static T InputOf<T>(string prompt)
    {
        var text = Input(prompt);
        var type = typeof(T);
        return (T)Convert.ChangeType(text, type);
    }

    public static async Task Choose(IList<(string, Func<Task>)> choices)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            var (text, _) = choices[i];
            WriteLine($"{i} {text}");
        }
        var choice = InputOf<int>("Choose");
        var action = choices[choice].Item2;
        await action();
    }
}
