namespace CardboardCore.DI
{
    /// <summary>
    /// Extends Unity's MonoBehaviour, automatically injects and releases any injected fields
    /// with `Inject` attribute
    /// </summary>
    public abstract class CardboardCoreBehaviour : UnityEngine.MonoBehaviour
    {
        protected enum InjectTiming
        {
            /// <summary>
            /// Release timing will be `OnDestroy`
            /// </summary>
            Awake,

            /// <summary>
            /// Release timing will be `OnDestroy`
            /// </summary>
            Start,

            /// <summary>
            /// Release timing will be `OnDisable`
            /// </summary>
            OnEnable
        };

        protected enum ReleaseTiming
        {
            OnDestroy,
            OnDisable
        }

        protected virtual InjectTiming MyInjectTiming => InjectTiming.Awake;
        protected ReleaseTiming MyReleaseTiming => MyInjectTiming == InjectTiming.OnEnable ? ReleaseTiming.OnDisable : ReleaseTiming.OnDestroy;

        protected abstract void OnInjected();
        protected abstract void OnReleased();

        protected virtual void Awake()
        {
            if (MyInjectTiming == InjectTiming.Awake)
            {
                Injector.Inject(this);
                OnInjected();
            }
        }

        protected virtual void Start()
        {
            if (MyInjectTiming == InjectTiming.Start)
            {
                Injector.Inject(this);
                OnInjected();
            }
        }

        protected virtual void OnEnable()
        {
            if (MyInjectTiming == InjectTiming.OnEnable)
            {
                Injector.Inject(this);
                OnInjected();
            }
        }

        protected virtual void OnDisable()
        {
            if (MyReleaseTiming == ReleaseTiming.OnDisable)
            {
                OnReleased();
                Injector.Release(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (MyReleaseTiming == ReleaseTiming.OnDestroy)
            {
                OnReleased();
                Injector.Release(this);
            }
        }
    }
}
