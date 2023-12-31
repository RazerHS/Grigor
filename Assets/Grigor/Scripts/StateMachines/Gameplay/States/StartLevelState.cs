using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Characters;
using Grigor.Gameplay.Interacting;
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

        private DataPodWidget dataPodWidget;
        private TimeOfDayWidget timeOfDayWidget;
        private MessagePopupWidget messagePopupWidget;

        protected override void OnEnter()
        {
            characterRegistry.Player.StartStateMachine();

            dataPodWidget = uiManager.ShowWidget<DataPodWidget>();
            timeOfDayWidget = uiManager.ShowWidget<TimeOfDayWidget>();
            messagePopupWidget = uiManager.ShowWidget<MessagePopupWidget>();

            interactablesRegistry.EnableInteractables();
            levelRegistry.DisableAllLevels();

            Level startLevel = levelRegistry.GetLevel(LevelName.Chongqing);
            startLevel.EnableLevel();

            characterRegistry.Player.Movement.MovePlayerToPosition(startLevel.SpawnPoint.position);

            owningStateMachine.ToNextState();
        }

        protected override void OnExit()
        {

        }
    }
}
