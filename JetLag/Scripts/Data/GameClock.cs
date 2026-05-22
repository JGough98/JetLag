namespace JetLag.Scripts.Data;

public class GameClock
{
    public static readonly TimeSpan DefaultTickInterval = TimeSpan.FromSeconds(30);
    public TimeSpan CurrentTime { get; private set; }
    public TimeSpan TickInterval { get; private set; } = DefaultTickInterval;
    public event Action? OnTick;

    public GameClock() { CurrentTime = TimeSpan.Zero; }

    public void Tick() { CurrentTime += TickInterval; OnTick?.Invoke(); }

    public void AdvanceBy(TimeSpan delta) { CurrentTime += delta; OnTick?.Invoke(); }

    // Deliberately does not fire OnTick — sets initial time without triggering reactions.
    public void Reset(TimeSpan startTime) { CurrentTime = startTime; }
}
