using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardboardCore.StateMachines
{
    /// <summary>
    /// Simple to use state machine. Create Transitions from State and to State and set an initial State before starting it.
    /// Use "ToNextState" to Transition to the next state, if this Transition is available.
    /// </summary>
    public abstract class StateMachine
    {
        protected readonly Dictionary<Type, State> stateDict = new Dictionary<Type, State>();
        protected readonly Dictionary<State, KeyValuePair<State, Transition>> staticTransitionDict = new Dictionary<State, KeyValuePair<State, Transition>>();
        protected readonly Dictionary<State, List<KeyValuePair<State, Transition>>> freeFlowTransitionDict = new Dictionary<State, List<KeyValuePair<State, Transition>>>();

        protected State initialState;
        protected State currentState;

        public bool EnableDebugging { get; private set; }

        public State CurrentState => currentState;

        public event Action StartedEvent;
        public event Action StoppedEvent;

        public StateMachine(bool enableDebugging)
        {
            EnableDebugging = enableDebugging;
        }

        /// <summary>
        /// Creates a new State, or gets an existing one if this state type already exists
        /// </summary>
        /// <typeparam name="T">State Class</typeparam>
        /// <returns>State Class</returns>
        private State CreateState<T>()
            where T : State, new()
        {
            Type type = typeof(T);

            if (stateDict.ContainsKey(type))
            {
                return stateDict[type];
            }

            T state = new T();
            stateDict[type] = state;

            return state;
        }

        /// <summary>
        /// Gets a state. Set "catchException" to true to fail if State does not exist
        /// </summary>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <typeparam name="T">State Class</typeparam>
        /// <returns>State Class</returns>
        private State GetState<T>(bool catchException = false)
            where T : State, new()
        {
            Type type = typeof(T);
            if (stateDict.ContainsKey(type))
            {
                return stateDict[type];
            }

            if (catchException)
            {
                throw new Exception($"State of type {type.Name} could not be found!");
            }
            else
            {
                return CreateState<T>();
            }
        }

        /// <summary>
        /// Get a Transition. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <typeparam name="TFrom">State Class</typeparam>
        /// <typeparam name="TTo">State Class</typeparam>
        /// <returns>Transition (Static)</returns>
        private Transition GetStaticTransition<TFrom, TTo>(bool catchException = false)
            where TFrom : State, new()
            where TTo : State, new()
        {
            State fromState = GetState<TFrom>(true);
            State toState = GetState<TTo>(true);

            return GetStaticTransition(fromState, toState, catchException);
        }

        /// <summary>
        /// Get a Transition, from the current State. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <typeparam name="TTo">State Class</typeparam>
        /// <returns>Transition (Static)</returns>
        private Transition GetStaticTransition<TTo>(bool catchException = false)
            where TTo : State, new()
        {
            State toState = GetState<TTo>(true);

            return GetStaticTransition(currentState, toState, catchException);
        }

        /// <summary>
        /// Get a Transition. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="from">State Class</param>
        /// <param name="to">State Class</param>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <returns>Transition (Static)</returns>
        private Transition GetStaticTransition(State from, State to, bool catchException = false)
        {
            if (staticTransitionDict.ContainsKey(from)
                && staticTransitionDict[from].Key == to)
            {
                return staticTransitionDict[from].Value;
            }
            
            // If the "from" State is already part of a static transition, no other transitions are allowed to use it.
            if (staticTransitionDict.ContainsKey(from))
            {
                throw new Exception($"State {from.GetType().Name} is already part of a static transition, no other transitions are allowed to use it.");
            }

            if (catchException)
            {
                string fromName = from.GetType().Name;
                string toName = to.GetType().Name;
                throw new Exception($"Transition from State {from.GetType().Name} "
                    + $"to State {to.GetType().Name} does not exist! Please create a transition with: \"AddTransition<{fromName},{toName}>()\"");
            }

            return null;
        }

        /// <summary>
        /// Get a FreeFlowTransition. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns>Transition (Free Flow)</returns>
        private Transition GetFreeFlowTransition<TFrom, TTo>(bool catchException = false)
            where TFrom : State, new()
            where TTo : State, new()
        {
            State fromState = GetState<TFrom>(true);
            State toState = GetState<TTo>(true);

            return GetFreeFlowTransition(fromState, toState, catchException);
        }

        /// <summary>
        /// Get a FreeFlowTransition, from the current State. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <typeparam name="TTo"></typeparam>
        /// <returns>Transition (Free Flow)</returns>
        private Transition GetFreeFlowTransition<TTo>(bool catchException = false)
            where TTo : State, new()
        {
            State toState = GetState<TTo>(true);

            return GetFreeFlowTransition(currentState, toState, catchException);
        }

        /// <summary>
        /// Get a FreeFlowTransition. Set "catchException" to true to fail if Transition does not exist
        /// </summary>
        /// <param name="from">State Class</param>
        /// <param name="to">State Class</param>
        /// <param name="catchException">Do we catch an exception when no transition is found? Or do we return "null" instead?</param>
        /// <returns>Transition (Free Flow)</returns>
        private Transition GetFreeFlowTransition(State from, State to, bool catchException = false)
        {
            if (freeFlowTransitionDict.ContainsKey(from))
            {
                for (int i = 0; i < freeFlowTransitionDict[from].Count; i++)
                {
                    if (freeFlowTransitionDict[from][i].Key == to)
                    {
                        return freeFlowTransitionDict[from][i].Value;
                    }
                }
            }

            if (catchException)
            {
                string fromName = from.GetType().Name;
                string toName = to.GetType().Name;
                throw new Exception($"FreeFlow Transition from State {fromName} "
                    + $"to State {toName} does not exist! Please create a transition with: \"AddFreeFlowTransition<{fromName},{toName}>()\"");
            }

            return null;
        }
        
        /// <summary>
        /// Add a static transition from State A to State B. Only one static transition from a specific state is allowed to be created.
        /// </summary>
        /// <typeparam name="TFrom">State Class</typeparam>
        /// <typeparam name="TTo">State Class</typeparam>
        /// <returns>To State Class</returns>
        public TTo AddStaticTransition<TFrom, TTo>()
            where TFrom : State, new()
            where TTo : State, new()
        {
            State from = CreateState<TFrom>();
            State to = CreateState<TTo>();

            Transition transition = GetStaticTransition<TFrom, TTo>();
            if (transition == null)
            {
                transition = new Transition(from, to);
                staticTransitionDict[from] = new KeyValuePair<State, Transition>(to, transition);
            }

            return to as TTo;
        }
        
        /// <summary>
        /// Add a transition from State A to State B
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns>To State Class</returns>
        public TTo AddFreeFlowTransition<TFrom, TTo>()
            where TFrom : State, new()
            where TTo : State, new()
        {
            State from = CreateState<TFrom>();
            State to = CreateState<TTo>();

            Transition transition = GetFreeFlowTransition<TFrom, TTo>();
            if (transition == null)
            {
                transition = new Transition(from, to);

                KeyValuePair<State, Transition> toAndTransition = new KeyValuePair<State, Transition>(to, transition);

                if (freeFlowTransitionDict.TryGetValue(from, out List<KeyValuePair<State, Transition>> list))
                {
                    list.Add(toAndTransition);
                }
                else
                {
                    freeFlowTransitionDict[from] = new List<KeyValuePair<State, Transition>> {toAndTransition};
                }
            }

            return to as TTo;
        }

        /// <summary>
        /// Set the initial State for the start of the State Machine. This is a requirement for every State Machine.
        /// </summary>
        /// <typeparam name="T">State Class</typeparam>
        public T SetInitialState<T>()
            where T : State, new()
        {
            initialState = GetState<T>();

            if (initialState == null)
            {
                initialState = CreateState<T>();
            }

            return initialState as T;
        }

        /// <summary>
        /// Start this state machine. Initial State will become active. StartedEvent will fire.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Start()
        {
            foreach (var item in stateDict)
            {
                item.Value.Initialize(this);
            }

            currentState = initialState ?? throw new Exception("Initial State is null! Please check if \"SetInitialState\" is used accordingly");
            currentState.Enter();

            StartedEvent?.Invoke();
        }

        /// <summary>
        /// Stop this state machine. Current state will be exited (if any). StoppedEvent will fire.
        /// </summary>
        public void Stop()
        {
            if (currentState == null)
            {
                return;
            }

            currentState.Exit();
            currentState = null;

            StoppedEvent?.Invoke();
        }

        /// <summary>
        /// Use FreeFlowTransition to move to the next state from current state. Note that a
        /// FreeFlowTransition must be made to use this method from a specific state. 
        /// </summary>
        /// <typeparam name="T">State Class</typeparam>
        public void ToState<T>()
            where T : State, new()
        {
            if (currentState == null)
            {
                Debug.LogWarning($"Trying to go to State {typeof(T).Name}, but there's no current state... Probably because the StateMachine has stopped.");
                return;
            }

            Transition transition = GetFreeFlowTransition<T>(true);
            transition.Do(out currentState);
        }

        /// <summary>
        /// Use StaticTransition to move to the next state from the current state. Note that a
        /// StaticTransition must be made to use this method from a specific state.
        /// </summary>
        public void ToNextState()
        {
            if (currentState == null)
            {
                Debug.LogWarning("Trying to go to next state, but there's no current state... Probably because the StateMachine has stopped.");
                return;
            }

            Transition transition = staticTransitionDict[currentState].Value;
            transition.Do(out currentState);
        }
    }
}
