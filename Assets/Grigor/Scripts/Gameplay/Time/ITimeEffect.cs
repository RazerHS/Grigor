namespace Grigor.Gameplay.Time
{
    public interface ITimeEffect
    {
        public void OnChangedToDay();
        public void OnChangedToNight();
        public void RegisterTimeEffect();
    }
}
