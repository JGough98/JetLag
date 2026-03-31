using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using Microsoft.AspNetCore.Components;


namespace JetLag.Scripts.Intialize;

public class MapLibreGameIntializer : IGameIntializer<MapLibre>
{
    private IMapMouseObserver _mapMouseObserver;

    private IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> _cardFactory;

    private MapUIController _mapUIController;

    private IReadOnlyList<QuestionCardModel> _cards = Array.Empty<QuestionCardModel>();


    public IReadOnlyList<QuestionCardModel> Cards => _cards;


    public MapLibreGameIntializer(
        IMapMouseObserver mapMouseObserverIntializer,
        IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> cardFactory,
        MapUIController mapUIController
    )
    {
        _mapMouseObserver = mapMouseObserverIntializer;
        _cardFactory = cardFactory;
        _mapUIController = mapUIController;
    }


    public async Task HandleMapLoaded(EventArgs args)
    {
        await _mapUIController.HandleMapLoaded(args);
    }

    public void Awake(IHandleEvent uiComponent)
    {
        _cards = _cardFactory.Create(
            new QuestionCardFactoryInput(uiComponent, _mapUIController.HandleQuestionButton)
        );
    }

    public async Task Intialize(MapLibre map, IHandleEvent uiComponent)
    {
        await _mapMouseObserver.Subscribe(map);

        _mapUIController.Intialize(map);

        _mapMouseObserver.OnClick = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapUIController.HandleClick
        );

        _mapMouseObserver.OnMouseMove = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapUIController.HandleMove
        );
    }

    public void Stop()
    {
        _mapMouseObserver.Unsubscribe();
    }

}