namespace JetLag.Scripts.Models;


public class QuestionButtonModel
{
    public string ImageSrc { get; init; } = string.Empty;

    public Func<Task> Command { get; init; } = () => Task.CompletedTask;
}


public class QuestionCardModel
{
    public string Title { get; init; } = string.Empty;
    public string SubTitle { get; init; } = string.Empty;
    public string Color { get; init; } = "#ffffff";
    public string MainImage { get; init; } = string.Empty;

    public QuestionButtonModel[] Buttons { get; init; } = Array.Empty<QuestionButtonModel>();
}
