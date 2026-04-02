using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Models;


namespace JetLag.Scripts.Mechanics.MapAction;

public class MapActionManager : IMapActionManager
{
    private bool _mapIsIntialized = false;

    private IMapAction? _currentMapAction;

    private QuestionButtonEventArgs? _currentQuestionButtonEventArgs;

    private IReadOnlyDictionary<string, IMapAction> _mapActions;


    private bool CantTriggerMapAction => !_mapIsIntialized || _currentMapAction == null;


    public MapActionManager(IReadOnlyList<IMapAction> mapActions)
    {
        _mapActions = mapActions.ToDictionary(k => k.Name, v => v);
    }


    public async Task HandleQuestionButton(QuestionButtonEventArgs questionButtonEventArgs)
    {
        if(!_mapActions.ContainsKey(questionButtonEventArgs.Title))
            return;

        if(_currentMapAction != null && _currentMapAction.Name == questionButtonEventArgs.Title)
        {
            _currentMapAction = null;
            return;
        }

        _currentQuestionButtonEventArgs = questionButtonEventArgs;
        _currentMapAction = _mapActions[questionButtonEventArgs.Title];
    }

    public async Task HandleClick(MapMouseEvent mapMouseEvent)
    {
        if (CantTriggerMapAction)
            return;

        await _currentMapAction!.HandleClick(mapMouseEvent, _currentQuestionButtonEventArgs!);
    }

    public async Task HandleMove(MapMouseEvent mapMouseEvent)
    {
        if (CantTriggerMapAction)
            return;

        await _currentMapAction!.HandleMove(mapMouseEvent, _currentQuestionButtonEventArgs!);
    }

    public async Task HandleMapLoaded(EventArgs args)
    {
        _mapIsIntialized = true;
    }
}