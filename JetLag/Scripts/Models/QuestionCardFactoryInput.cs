using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Models;

public record QuestionCardFactoryInput(
    IHandleEvent Component,
    Func<QuestionButtonEventArgs, Task> OnButtonTapped
);
