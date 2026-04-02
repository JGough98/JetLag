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

        private bool _originSet = false;
        private double _originLat;
        private double _originLng;
        private double _currentAngle = 0;


        public string Name => "THERMOMETER";


        public ThermometerMapAction(MapRender mapRender, IHiderProxy hiderProxy)
        {
            _mapRender = mapRender;
            _hiderProxy = hiderProxy;
        }


        public async Task HandleClick(
            MapMouseEvent mapMouseEvent,
            QuestionButtonEventArgs questionButtonEventArgs
        )
        {
            if (!_originSet)
            {
                _originLat = mapMouseEvent.LngLat.Latitude;
                _originLng = mapMouseEvent.LngLat.Longitude;
                _currentAngle = 0;
                _originSet = true;

                await _mapRender.Overlay.RenderStraightLine(_originLat, _originLng, _currentAngle);
            }
            else
            {
                // If the hider is on the drawn side, flip 180° so OutOfBounds shows
                // the side where the hider is NOT.
                var hiderOnDrawnSide = await _hiderProxy.LineHitHider(_originLat, _originLng, _currentAngle);
                double finalAngle = hiderOnDrawnSide ? _currentAngle + 180 : _currentAngle;

                await _mapRender.OutOfBounds.RenderStraightLine(_originLat, _originLng, finalAngle);
                await _mapRender.Overlay.Clear();
                _originSet = false;
            }
        }

        public async Task HandleMove(
            MapMouseEvent mapMouseEvent,
            QuestionButtonEventArgs questionButtonEventArgs
        )
        {
            if (!_originSet)
                return;

            double dLng = mapMouseEvent.LngLat.Longitude - _originLng;
            double dLat = mapMouseEvent.LngLat.Latitude - _originLat;

            // Bearing from origin to cursor (degrees clockwise from north).
            // Moving right of origin increases angle; moving left decreases it.
            _currentAngle = Math.Atan2(dLng, dLat) * 180.0 / Math.PI;

            await _mapRender.Overlay.ReplaceStraightLine(_originLat, _originLng, _currentAngle);
        }

        public Task HandleLeave(
            MapMouseEvent mapMouseEvent,
            QuestionButtonEventArgs questionButtonEventArgs
            ) => _mapRender.Overlay.Clear();
    }
}
