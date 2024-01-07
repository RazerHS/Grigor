using CardboardCore.DI;
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

        private DataPodWidget dataPodWidget;
        private TasksWidget tasksWidget;

        protected override void OnShow()
        {
            messagesButton.onClick.AddListener(OnMessagesButtonClicked);
            tasksButton.onClick.AddListener(OnTasksButtonClicked);
            dataPodButton.onClick.AddListener(OnDataPodButtonClicked);

            dataPodWidget = uiManager.GetWidget<DataPodWidget>();
            tasksWidget = uiManager.GetWidget<TasksWidget>();
        }

        protected override void OnHide()
        {
            messagesButton.onClick.RemoveListener(OnMessagesButtonClicked);
            tasksButton.onClick.RemoveListener(OnTasksButtonClicked);
            dataPodButton.onClick.RemoveListener(OnDataPodButtonClicked);
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
        }

        private void HideAll()
        {
            dataPodWidget.HideDataPod();
            tasksWidget.Hide();
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
