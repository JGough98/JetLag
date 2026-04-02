using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using Microsoft.AspNetCore.Components;

using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;
using JetLag.Scripts.Mechanics.MapAction;


namespace JetLag.Scripts.Intialize;

public class MapLibreOrchestrator : IMapOrchestrator<MapLibre>
{
    private readonly IMapMouseObserver _mapMouseObserver;

    private readonly IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> _cardFactory;

    private readonly IMapActionManager _mapActionManager;

    private readonly MapRender _mapRender;

    private IReadOnlyList<QuestionCardModel> _cards = Array.Empty<QuestionCardModel>();


    public IReadOnlyList<QuestionCardModel> Cards => _cards;


    public MapLibreOrchestrator(
        IMapMouseObserver mapMouseObserver,
        IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> cardFactory,
        IMapActionManager mapActionManager,
        MapRender mapRender
    )
    {
        _mapMouseObserver = mapMouseObserver;
        _cardFactory = cardFactory;
        _mapActionManager = mapActionManager;
        _mapRender = mapRender;
    }


    public void Initialize(IHandleEvent uiComponent)
    {
        _cards = _cardFactory.Create(
            new QuestionCardFactoryInput(uiComponent, _mapActionManager.HandleQuestionButton)
        );
    }

    public async Task MapLoaded(MapLibre map, IHandleEvent uiComponent, EventArgs args)
    {
        await _mapMouseObserver.Subscribe(map);

        _mapRender.Intialize(map);

        _mapMouseObserver.OnClick = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleClick
        );

        _mapMouseObserver.OnMouseMove = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleMove
        );

        await _mapActionManager.HandleMapLoaded(args);
    }

    public void Stop()
    {
        _mapMouseObserver.Unsubscribe();
    }
}