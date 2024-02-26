using System.Collections.Generic;
using Grigor.Data.Tasks;
using Grigor.UI.Data;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class TasksWidget : UIWidget
    {
        [SerializeField] private TaskUIDisplay taskUIDisplayPrefab;
        [SerializeField] private Transform taskUIDisplayParent;
        [SerializeField] private TextMeshProUGUI selectedTaskName;
        [SerializeField] private TextMeshProUGUI selectedTaskDescription;

        [ShowInInspector] private List<TaskUIDisplay> displayedTasks = new();

        protected override void OnShow()
        {
            foreach (TaskUIDisplay taskUIDisplay in displayedTasks)
            {
                taskUIDisplay.OnSelectedTask += OnTaskSelected;
            }

            selectedTaskName.text = "Select a task!";
            selectedTaskDescription.text = "";
        }

        protected override void OnHide()
        {
            foreach (TaskUIDisplay taskUIDisplay in displayedTasks)
            {
                taskUIDisplay.OnSelectedTask -= OnTaskSelected;
            }
        }

        public void OnTaskCompleted(TaskData taskData)
        {
            TaskUIDisplay taskUIDisplay = displayedTasks.Find(x => x.TaskData == taskData);

            if (taskUIDisplay == null)
            {
                return;
            }

            displayedTasks.Remove(taskUIDisplay);

            Destroy(taskUIDisplay.gameObject);
        }

        public void OnTaskStarted(TaskData taskData)
        {
            TaskUIDisplay taskUIDisplay = Instantiate(taskUIDisplayPrefab, transform);

            taskUIDisplay.transform.SetParent(taskUIDisplayParent);

            taskUIDisplay.Initialize(taskData);

            if (displayedTasks.Contains(taskUIDisplay))
            {
                return;
            }

            displayedTasks.Add(taskUIDisplay);
        }

        private void OnTaskSelected(TaskData taskData)
        {
            selectedTaskName.text = taskData.TaskName;
            selectedTaskDescription.text = taskData.TaskDescription;
        }
    }
}
