using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Factory
{
    public class QuestionCardModelFactory : IFactory<IReadOnlyList<QuestionCardModel>, IHandleEvent>
    {
        // Once the uiComponent task is completed the uiComponent will auto refresh on the page.
        public IReadOnlyList<QuestionCardModel> Create(IHandleEvent uiComponent)
        {
            var eventCallBack =  EventCallback.Factory.Create<QuestionButtonEventArgs>(
                uiComponent,
                () => Task.CompletedTask);


            return new List<QuestionCardModel>()
                {
                    new QuestionCardModel
                    {
                        Title = "THERMOMETER",
                        SubTitle = "DRAW 2, PICK 1",
                        Color = "#f59e0b",
                        MainImage = "",
                        Buttons =
                        [
                            new QuestionButtonModel ("", new QuestionButtonEventArgs(){ Size = 10, Title = ""}, eventCallBack),
                        ]
                    },
                    new QuestionCardModel
                    {
                        Title = "RADAR",
                        SubTitle = "DRAW 2, PICK 1",
                        Color = "#f97316",
                        MainImage = "",
                        Buttons =
                        [
                            new QuestionButtonModel ("", new QuestionButtonEventArgs(){ Size = 10, Title = ""}, eventCallBack),
                        ]
                    }
                };
        }
    }
}