namespace TicTacToe.Console;

public interface IClientNotification
{
    public void ShowInfo(string message);
    public void ShowError(string message);
}
