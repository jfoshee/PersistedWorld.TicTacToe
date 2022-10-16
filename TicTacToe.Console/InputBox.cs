namespace TicTacToe.Console;

public static class InputBox
{
    public static string? Show(string prompt, string? title, string? defaultValue)
    {
        bool okPressed = false;

        var ok = new Button("Ok", is_default: true);
        ok.Clicked += () => { okPressed = true; Application.RequestStop(); };
        var cancel = new Button("Cancel");
        cancel.Clicked += () => { Application.RequestStop(); };
        var dialog = new Dialog(title, ok, cancel);

        var label = new Label(prompt)
        {
            X = 0,
            Y = 1
        };

        var textField = new TextField(defaultValue ?? "")
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill()
        };

        dialog.Add(label, textField);
        textField.SetFocus();

        Application.Run(dialog);

        if (okPressed)
        {
            return textField.Text.ToString();
        }
        return null;
    }
}
