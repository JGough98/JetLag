using Microsoft.AspNetCore.Components;

namespace JetLag.Scripts.Models;


public class QuestionButtonModel
{
    public string ImageSrc { get; init; } = string.Empty;

    public QuestionButtonEventArgs Data { get; init; }

    public EventCallback<QuestionButtonEventArgs> OnTapped { get; init; }


    public QuestionButtonModel(
        string imageSrc,
        QuestionButtonEventArgs args,
        EventCallback<QuestionButtonEventArgs> callback
    )
    {
        Data = args;
        ImageSrc = imageSrc;
        OnTapped = callback;
    }


    public async Task InvokeAsync()
    {
        if (OnTapped.HasDelegate)
        {
            await OnTapped.InvokeAsync(Data);
        }
    }
}
