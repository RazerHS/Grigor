using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Tasks;
using Grigor.Gameplay.Tasks;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class BeginTaskInteractable : InteractableComponent
    {
        [SerializeField] private TaskData taskToBegin;

        [Inject] private TaskManager taskManager;

        public TaskData TaskToBegin => taskToBegin;

        protected override void OnInitialized()
        {
            if (taskToBegin == null)
            {
                throw Log.Exception($"Task data is not set for <b>{name}</b>!");
            }
        }

        protected override void OnInteractEffect()
        {
            taskManager.StartTask(taskToBegin);

            EndInteract();
        }
    }
}
