using Community.Blazor.MapLibre;
using Community.Blazor.MapLibre.Models.Event;
using Microsoft.AspNetCore.Components;

using JetLag.Scripts.Data;
using JetLag.Scripts.Factory.Interface;
using JetLag.Scripts.Input;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;
using JetLag.Scripts.Transit;
using JetLag.Scripts.Mechanics.MapAction;


namespace JetLag.Scripts.Intialize;

public class MapLibreOrchestrator : IMapOrchestrator<MapLibre>
{
    private readonly IMapMouseObserver _mapMouseObserver;

    private readonly IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> _cardFactory;

    private readonly IMapActionManager _mapActionManager;

    private readonly MapRender _mapRender;

    private readonly RailwayLayerRender _railwayLayerRender;

    private readonly StationLayerRender _stationLayerRender;

    private readonly TripPathLayerRender _tripPathLayerRender;

    private readonly PlanningState _planningState;

    private readonly ITransitService _transitService;


    public IReadOnlyList<QuestionCardModel> Cards { get; private set; } = Array.Empty<QuestionCardModel>();


    public MapLibreOrchestrator(
        IMapMouseObserver mapMouseObserver,
        IFactory<IReadOnlyList<QuestionCardModel>, QuestionCardFactoryInput> cardFactory,
        IMapActionManager mapActionManager,
        MapRender mapRender,
        RailwayLayerRender railwayLayerRender,
        StationLayerRender stationLayerRender,
        TripPathLayerRender tripPathLayerRender,
        PlanningState planningState,
        ITransitService transitService
    )
    {
        _mapMouseObserver = mapMouseObserver;
        _cardFactory = cardFactory;
        _mapActionManager = mapActionManager;
        _mapRender = mapRender;
        _railwayLayerRender = railwayLayerRender;
        _stationLayerRender = stationLayerRender;
        _tripPathLayerRender = tripPathLayerRender;
        _planningState = planningState;
        _transitService = transitService;
    }


    public void Initialize(IHandleEvent uiComponent)
    {
        Cards = _cardFactory.Create(
            new QuestionCardFactoryInput(uiComponent, _mapActionManager.HandleQuestionButton)
        );
    }

    public async Task MapLoaded(MapLibre map, IHandleEvent uiComponent, EventArgs args)
    {
        await _mapMouseObserver.Subscribe(map);

        await _railwayLayerRender.InitializeAsync(map);
        await _stationLayerRender.InitializeAsync(map);
        _tripPathLayerRender.Initialize(map);

        _mapRender.Intialize(map);

        _planningState.OnChange += () =>
        {
            _ = _planningState.SelectedTripId is not null
                ? _tripPathLayerRender.DrawTripPath(_planningState.SelectedTripId)
                : _tripPathLayerRender.Clear();
        };

        _mapMouseObserver.OnMouseMove = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleMove
        );
        _mapMouseObserver.OnMouseLeave = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            _mapActionManager.HandleLeave
        );
        _mapMouseObserver.OnClick = EventCallback.Factory.Create<MapMouseEvent>(
            uiComponent,
            (MapMouseEvent e) => HandleClickWithStationDetection(e)
        );

        await _mapActionManager.HandleMapLoaded(args);
    }

    public void Stop()
    {
        _mapMouseObserver.Unsubscribe();
    }


    private async Task HandleClickWithStationDetection(MapMouseEvent e)
    {
        var nearest = _stationLayerRender.FindNearestStopWithinRadius(
            e.LngLat.Latitude,
            e.LngLat.Longitude
        );

        if (nearest is not null)
        {
            await _stationLayerRender.HighlightStop(nearest);
            var now = DateTime.Now.TimeOfDay;
            var departures = await _transitService.GetDepartingTrains(
                nearest.StopId,
                now,
                now.Add(TimeSpan.FromHours(2))
            );
            _planningState.SelectStop(nearest, departures);
            return;
        }

        await _mapActionManager.HandleClick(e);
    }
}