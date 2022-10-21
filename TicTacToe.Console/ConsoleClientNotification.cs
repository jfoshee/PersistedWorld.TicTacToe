namespace TicTacToe.Console;

using Monxterz.StatePlatform.Client;
using static System.Console;

public class ConsoleClientNotification : IClientNotification
{
    public void ShowInfo(string message)
    {
        WriteLine(message);
    }

    public void ShowError(string message)
    {
        WriteLine(message);
    }

    public void ShowError(ApiException apiException)
    {
        ShowError(apiException.SimpleMessage());
    }
}
