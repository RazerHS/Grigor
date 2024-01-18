using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters.Components;
using Grigor.Data.Tasks;
using Grigor.Gameplay.Audio;
using Grigor.Gameplay.Time;
using Grigor.Input;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class InteractableComponent : MonoBehaviour
    {
        [Tooltip("Should this component be triggered when the player is in range?")]
        [SerializeField, ColoredBoxGroup("Config", false, true)] protected bool interactInRange;

        [Tooltip("Will this component remove itself from the chain so that it cannot be interacted with again after the first interaction?")]
        [SerializeField, ColoredBoxGroup("Chain", false, true), InfoBox("Stays in chain and also stops the chain!", InfoMessageType.Warning, nameof(CheckConfig))] protected bool removeFromChainAfterEffect;

        [Tooltip("Should this interaction stop the interaction chain and let the player roam?")]
        [SerializeField, ColoredBoxGroup("Chain"), HideIf(nameof(IsOnlyInteractableInChain))] protected bool stopsChain;

        [Tooltip("The element index of this component in the parent interactable chain. You can change this index by clicking the arrows to the right of each element in the chain.")]
        [SerializeField, ColoredBoxGroup("Chain"), HideIf(nameof(IsOnlyInteractableInChain)), ReadOnly] protected int indexInChain;

        [Tooltip("Does this component have an effect when the time of day changes to day and night?")]
        [SerializeField, ColoredBoxGroup("Time", false, true)] protected bool hasTimeEffect;

        [Tooltip("Should time pass after this interaction triggers?")]
        [SerializeField, ColoredBoxGroup("Time")] protected bool timePassesOnInteract;

        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 60)] protected int minutesToPass = 1;
        [SerializeField, ColoredBoxGroup("Time"), ShowIf(nameof(timePassesOnInteract)), Range(0, 24)] protected int hoursToPass = 1;

        [SerializeField, ColoredBoxGroup("Tasks", false, true)] protected bool stopChainIfTaskNotStarted;
        [SerializeField, ColoredBoxGroup("Tasks"), ShowIf(nameof(stopChainIfTaskNotStarted))] protected TaskData taskToListenTo;

        [SerializeField, ColoredBoxGroup("Audio", false, true)] protected bool playAudio;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf(nameof(playAudio))] protected bool playAmbienceAudio;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf(nameof(playAudio)), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string interactAudio;
        [SerializeField, ColoredBoxGroup("Audio"), ShowIf("@playAudio && playAmbienceAudio"), ValueDropdown("@GameConfig.Instance.GetAudioEvents()")] protected string ambienceAudio;

        [SerializeField, ColoredBoxGroup("Debug", false, true), ReadOnly] private bool interactionEnabled;
        [ShowInInspector, ColoredBoxGroup("Debug", false, true), ReadOnly] private bool interactedWithInCurrentChain;

        [Inject] private TimeManager timeManager;
        [Inject] private AudioController audioController;
        [Inject] private PlayerInput playerInput;

        protected Interactable parentInteractable;
        protected bool stillInRangeAfterInteraction;
        protected bool currentlyInteracting;

        public bool InteractInRange => interactInRange;
        public bool CurrentlyInteracting => currentlyInteracting;
        public bool StillInRangeAfterInteraction => stillInRangeAfterInteraction;
        public bool StopsChain => stopsChain;
        public bool RemoveFromChainAfterEffect => removeFromChainAfterEffect;
        public int IndexInChain => indexInChain;
        public Interactable ParentInteractable => parentInteractable;
        public bool InteractionEnabled => interactionEnabled;
        public bool InteractedWithInCurrentChain => interactedWithInCurrentChain;
        public TaskData TaskToListenTo => taskToListenTo;
        public bool StopChainIfTaskNotStarted => stopChainIfTaskNotStarted;

        // NOTE: necessary because DI does not support inheritance, so this is a workaround to not inject the same object twice into children
        public TimeManager TimeManager => timeManager;
        public AudioController AudioController => audioController;

        public event Action BeginInteractionEvent;
        public event Action EndInteractionEvent;

        private bool IsOnlyInteractableInChain()
        {
            return parentInteractable.InteractablesChain.InteractablesInChainCount <= 1;
        }

        private bool CheckConfig()
        {
            return stopsChain && !removeFromChainAfterEffect;
        }

        [OnInspectorInit]
        protected virtual void OnInspectorInit()
        {
            FindInteractable();
        }

        public void Initialize()
        {
            Injector.Inject(this);

            if (parentInteractable == null)
            {
                parentInteractable = GetComponent<Interactable>();
            }

            parentInteractable.InRangeEvent += OnInRange;
            parentInteractable.OutOfRangeEvent += OnOutOfRange;

            if (hasTimeEffect)
            {
                timeManager.ChangedToDayEvent += OnChangedToDay;
                timeManager.ChangedToNightEvent += OnChangedToNight;

                Log.Write($"Registering to time manager: {name}");
            }

            if (playAudio && playAmbienceAudio)
            {
                audioController.PlaySound3D(ambienceAudio, transform);
            }

            OnInitialized();
        }

        protected virtual void OnChangedToNight() { }

        protected virtual void OnChangedToDay() { }

        public void Dispose()
        {
            parentInteractable.InRangeEvent -= OnInRange;
            parentInteractable.OutOfRangeEvent -= OnOutOfRange;
            parentInteractable.InteractEvent -= OnInteract;

            if (hasTimeEffect)
            {
                timeManager.ChangedToDayEvent -= OnChangedToDay;
                timeManager.ChangedToNightEvent -= OnChangedToNight;
            }

            OnDisposed();

            Injector.Release(this);
        }

        protected virtual void OnInitialized() {}

        protected virtual void OnDisposed() {}

        protected void FindInteractable()
        {
            if (parentInteractable == null)
            {
                parentInteractable = GetComponent<Interactable>();
            }
        }

        public void OnInRange(Character character)
        {
            InRangeEffect();
        }

        private void OnOutOfRange(Character character)
        {
            stillInRangeAfterInteraction = false;

            OutOfRangeEffect();
        }

        protected virtual void InRangeEffect() { }

        protected virtual void OutOfRangeEffect() { }

        private void OnInteract()
        {
            BeginInteractionEvent?.Invoke();

            currentlyInteracting = true;

            EnableSkipInput();

            if (playAudio)
            {
                audioController.PlaySound3D(interactAudio, transform);
            }

            OnInteractEffect();
        }

        protected virtual void OnInteractEffect() { }

        protected void EndInteract()
        {
            if (timePassesOnInteract)
            {
                timeManager.PassTime(minutesToPass, hoursToPass);
            }

            if (parentInteractable.InRange)
            {
                stillInRangeAfterInteraction = true;
            }

            currentlyInteracting = false;

            interactedWithInCurrentChain = true;

            DisableSkipInput();

            EndInteractionEvent?.Invoke();

            EndInteractEffect();
        }

        protected virtual void EndInteractEffect() { }

        public void EnableInteraction()
        {
            if (!interactionEnabled)
            {
               parentInteractable.InteractEvent += OnInteract;
            }

            interactionEnabled = true;
        }

        public void DisableInteraction()
        {
            if (interactionEnabled)
            {
                parentInteractable.InteractEvent -= OnInteract;
            }

            interactionEnabled = false;
        }

        public void OnCurrentChainEnded()
        {
            interactedWithInCurrentChain = false;

            EnableInteraction();
        }

        protected void ResetInteraction()
        {
            EnableInteraction();

            parentInteractable.ResetInteractable();
        }

        public void SetDefaultIndexInChain(int index)
        {
            indexInChain = index;
        }

        private void EnableSkipInput()
        {
            playerInput.InteractInputStartedEvent += OnSkipInputDuringInteraction;
            playerInput.SkipInputStartedEvent += OnSkipInputDuringInteraction;
        }

        private void DisableSkipInput()
        {
            playerInput.InteractInputStartedEvent -= OnSkipInputDuringInteraction;
            playerInput.SkipInputStartedEvent -= OnSkipInputDuringInteraction;
        }

        /// <summary>
        /// Used to listen for the press of the interact key while interacting.
        /// </summary>
        protected virtual void OnSkipInputDuringInteraction() { }

        public bool StopChainIfRequiredTaskNotStarted()
        {
            if (!stopChainIfTaskNotStarted)
            {
                return false;
            }

            if (taskToListenTo.Started)
            {
                EnableInteraction();

                return false;
            }

            DisableInteraction();

            return true;
        }
    }
}
