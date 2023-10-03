using CardboardCore.DI;
using DG.Tweening;
using Grigor.Overworld.Rooms;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Widgets
{
    public class TimeOfDayToggleWidget : UIWidget
    {
        [SerializeField] private Button timeOfDayToggleButton;
        [SerializeField] private float dayStartTime = 12f;
        [SerializeField] private float nightStartTime = 24f;
        [SerializeField] private float transitionDuration = 3f;

        [Inject] private RoomRegistry roomRegistry;

        [ShowInInspector, ReadOnly] private bool isDayTime;
        [ShowInInspector, ReadOnly] private float currentTimeOfDay;
        private Room currentRoom;
        private RoomNames currentRoomName;

        protected override void OnShow()
        {
            currentRoom = roomRegistry.GetRoom(currentRoomName);

            currentTimeOfDay = currentRoom.Lighting.TimeOfDay;

            CheckTimeOfDay();

            timeOfDayToggleButton.onClick.AddListener(OnTimeOfDayToggleButtonClicked);
        }

        protected override void OnHide()
        {

        }

        private void OnTimeOfDayToggleButtonClicked()
        {
            float targetTimeOfDay = isDayTime ? nightStartTime : dayStartTime;

            DOVirtual.Float(currentTimeOfDay, targetTimeOfDay, transitionDuration, SetTimeOfDay);
        }

        private void SetTimeOfDay(float timeOfDay)
        {
            currentTimeOfDay = currentRoom.Lighting.SetTimeOfDay(timeOfDay);

            // TO-DO: fix time of day math

            CheckTimeOfDay();
        }

        private void CheckTimeOfDay()
        {
            isDayTime = currentTimeOfDay < 12f;
        }

        public void SetCurrentRoomType(RoomNames roomName)
        {
            currentRoomName = roomName;
        }
    }
}
