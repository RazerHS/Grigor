using CardboardCore.DI;
using Grigor.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class PhoneWidget : UIWidget
    {
        [SerializeField] private Button messagesButton;
        [SerializeField] private Button tasksButton;
        [SerializeField] private Button dataPodButton;

        [Inject] private UIManager uiManager;
        [Inject] private CharacterRegistry characterRegistry;

        private DataPodWidget dataPodWidget;
        private TasksWidget tasksWidget;
        private MessagesWidget messagesWidget;
        private bool active;

        public bool Active => active;

        protected override void OnShow()
        {
            messagesButton.onClick.AddListener(OnMessagesButtonClicked);
            tasksButton.onClick.AddListener(OnTasksButtonClicked);
            dataPodButton.onClick.AddListener(OnDataPodButtonClicked);

            dataPodWidget = uiManager.GetWidget<DataPodWidget>();
            tasksWidget = uiManager.GetWidget<TasksWidget>();
            messagesWidget = uiManager.GetWidget<MessagesWidget>();

            active = true;

            DisablePlayerControls();
        }

        protected override void OnHide()
        {
            messagesButton.onClick.RemoveListener(OnMessagesButtonClicked);
            tasksButton.onClick.RemoveListener(OnTasksButtonClicked);
            dataPodButton.onClick.RemoveListener(OnDataPodButtonClicked);

            active = false;

            EnablePlayerControls();
        }

        private void EnablePlayerControls()
        {
            if (characterRegistry == null)
            {
                return;
            }

            if (characterRegistry.Player == null)
            {
                return;
            }

            characterRegistry.Player.Movement.EnableMovement();
            characterRegistry.Player.Look.EnableLook();
            characterRegistry.Player.Interact.EnableInteract();
        }

        private void DisablePlayerControls()
        {
            characterRegistry.Player.Movement.DisableMovement();
            characterRegistry.Player.Look.DisableLook();
            characterRegistry.Player.Interact.DisableInteract();
        }

        private void OnDataPodButtonClicked()
        {
            HideAll();

           dataPodWidget.ShowDataPod();
        }

        private void OnTasksButtonClicked()
        {
            HideAll();

            tasksWidget.Show();
        }

        private void OnMessagesButtonClicked()
        {
            HideAll();

            messagesWidget.Show();
        }

        private void HideAll()
        {
            dataPodWidget.HideDataPod();
            tasksWidget.Hide();
            messagesWidget.Hide();
        }

        public void TogglePhone()
        {
            if (gameObject.activeSelf)
            {
                Hide();

                return;
            }

            Show();
        }
    }
}
