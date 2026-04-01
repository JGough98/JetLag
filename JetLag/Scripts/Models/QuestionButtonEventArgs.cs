namespace JetLag.Scripts.Models;

public class QuestionButtonEventArgs : EventArgs
{
    public required string Title { get; init; } = string.Empty;

    public required int Size { get; init; } = 0;
}
