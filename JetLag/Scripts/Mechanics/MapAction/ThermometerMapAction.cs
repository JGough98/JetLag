using Community.Blazor.MapLibre.Models.Event;
using JetLag.Scripts.Mechanics.Hider;
using JetLag.Scripts.Models;
using JetLag.Scripts.Render;


namespace JetLag.Scripts.Mechanics.MapAction
{
    public class ThermometerMapAction : IMapAction
    {
        private readonly MapRender _mapRender;

        private readonly IHiderProxy _hiderProxy;


        public string Name => "THEREMOMETER";


        public ThermometerMapAction(MapRender mapRender, IHiderProxy hiderProxy)
        {
            _mapRender = mapRender;
            _hiderProxy = hiderProxy;
        }


        public Task HandleClick(
            MapMouseEvent mapMouseEvent,
            QuestionButtonEventArgs questionButtonEventArgs
        )
        {
            throw new NotImplementedException();
        }

        public Task HandleMove(
            MapMouseEvent mapMouseEvent,
            QuestionButtonEventArgs questionButtonEventArgs
        )
        {
            throw new NotImplementedException();
        }
    }
}
