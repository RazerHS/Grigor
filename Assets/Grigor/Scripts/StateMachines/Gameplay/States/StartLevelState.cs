using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Gameplay.Interacting;
using Grigor.Gameplay.Settings;
using Grigor.Gameplay.Time;
using Grigor.UI;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.Gameplay.States
{
    public class StartLevelState : State
    {
        [Inject] private LevelRegistry levelRegistry;
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private InteractablesRegistry interactablesRegistry;
        [Inject] private UIManager uiManager;
        [Inject] private TimeManager timeManager;
        [Inject] private SettingsManager settingsManager;

        private DataPodWidget dataPodWidget;
        private TimeOfDayWidget timeOfDayWidget;
        private MessagePopupWidget messagePopupWidget;
        private ReceiveMessageWidget receiveMessageWidget;

        protected override void OnEnter()
        {
            characterRegistry.Player.StartStateMachine();

            interactablesRegistry.EnableInteractables();
            levelRegistry.DisableAllLevels();

            Level startLevel = levelRegistry.GetLevel(LevelName.Chongqing);
            levelRegistry.SetCurrentLevel(startLevel);

            startLevel.EnableLevel();

            characterRegistry.Player.Movement.MovePlayerToPosition(startLevel.SpawnPoint.position);

            dataPodWidget = uiManager.ShowWidget<DataPodWidget>();
            timeOfDayWidget = uiManager.ShowWidget<TimeOfDayWidget>();
            messagePopupWidget = uiManager.ShowWidget<MessagePopupWidget>();
            receiveMessageWidget = uiManager.ShowWidget<ReceiveMessageWidget>();

            settingsManager.OnQualityChanged(0);

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
