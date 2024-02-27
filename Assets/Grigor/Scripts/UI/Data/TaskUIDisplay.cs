using System;
using Grigor.Data.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    public class TaskUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI taskNameText;
        [SerializeField] private Button selectTaskButton;

        private TaskData taskData;

        public TaskData TaskData => taskData;

        public event Action<TaskData> OnSelectedTask;

        public void Initialize(TaskData taskData)
        {
            this.taskData = taskData;

            SetTaskName(taskData.TaskName);

            selectTaskButton.onClick.AddListener(OnSelectedTaskButton);
        }

        ~TaskUIDisplay()
        {
            selectTaskButton.onClick.RemoveListener(OnSelectedTaskButton);
        }

        public void SetTaskName(string taskName)
        {
            taskNameText.text = taskName;
        }

        private void OnSelectedTaskButton()
        {
            OnSelectedTask?.Invoke(taskData);
        }
    }
}
