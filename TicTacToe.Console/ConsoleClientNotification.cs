namespace TicTacToe.Console;
using static System.Console;

public class ConsoleClientNotification : IClientNotification
{
    // TODO: ShowError(Exception)

    public void ShowError(string message)
    {
        WriteLine(message);
    }

    public void ShowInfo(string message)
    {
        WriteLine(message);
    }
}
