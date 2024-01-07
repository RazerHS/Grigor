using System.Collections.Generic;
using Grigor.Data.Tasks;
using Grigor.UI.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class TasksWidget : UIWidget
    {
        [SerializeField] private TaskUIDisplay taskUIDisplayPrefab;
        [SerializeField] private Transform taskUIDisplayParent;

        [ShowInInspector] private List<TaskUIDisplay> displayedTasks = new();

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

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
    }
}
