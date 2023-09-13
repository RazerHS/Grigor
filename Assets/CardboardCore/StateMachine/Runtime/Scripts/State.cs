using System;

#if CC_DI
using CardboardCore.DI;
using CardboardCore.Utilities;
#endif

namespace CardboardCore.StateMachines
{
    /// <summary>
    /// Core State used by the StateMachine. Extend from this class to create your own State.
    /// </summary>
    public abstract class State
    {
        protected StateMachine owningStateMachine;

        protected virtual void OnInitialize(StateMachine owningStateMachine) { }
        protected abstract void OnEnter();
        protected abstract void OnExit();

        protected bool isActive;

        public void Initialize(StateMachine owningStateMachine)
        {
            this.owningStateMachine = owningStateMachine;
            OnInitialize(owningStateMachine);
        }

        public void Enter()
        {
            if (owningStateMachine.EnableDebugging)
            {
#if CC_DI
                Log.Write(GetType().Name);
#else
                UnityEngine.Debug.Log($"State Enter: {GetType().Name}");
#endif
            }

#if CC_DI
            Injector.Inject(this);
#endif

            isActive = true;
            OnEnter();
        }

        public void Exit()
        {
            if (owningStateMachine.EnableDebugging)
            {
#if CC_DI
                Log.Write(GetType().Name);
#else
                UnityEngine.Debug.Log($"State Exit: {GetType().Name}");
#endif

            }

            isActive = false;
            OnExit();

#if CC_DI
            Injector.Release(this);
#endif
        }
    }

    /// <summary>
    /// A State which will have it's owning StateMachine field mapped to the type given as T.
    /// Use this State if you need direct references to your state machine within states.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class State<T> : State where T : StateMachine
    {
        protected new T owningStateMachine;

        protected override void OnInitialize(StateMachine owningStateMachine)
        {
            if (owningStateMachine.GetType() != typeof(T))
            {
#if CC_DI
                throw Log.Exception($"Owning state machine is not of expected type {typeof(T).Name}");
#else
                throw new Exception($"Owning state machine is not of expected type {typeof(T).Name}");
#endif
            }

            this.owningStateMachine = (T)owningStateMachine;
        }
    }
}
