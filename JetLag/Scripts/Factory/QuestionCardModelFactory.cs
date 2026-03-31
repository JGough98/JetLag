using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Factory
{
    public class QuestionCardModelFactory : IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput>
    {
        public IReadOnlyList<QuestionCardModel> Create(QuestionCardFactoryInput input)
        {
            var eventCallBack = EventCallback.Factory.Create<QuestionButtonEventArgs>(
                input.Component,
                (args) => input.OnButtonTapped(args));


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
                            new QuestionButtonModel ("", new QuestionButtonEventArgs(){ Size = 10, Title = "THERMOMETER"}, eventCallBack),
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
                            new QuestionButtonModel ("", new QuestionButtonEventArgs(){ Size = 10, Title = "RADAR"}, eventCallBack),
                        ]
                    }
                };
        }
    }
}
