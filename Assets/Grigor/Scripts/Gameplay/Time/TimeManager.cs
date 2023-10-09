using System;
using CardboardCore.DI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    [Injectable]
    public class TimeManager : MonoBehaviour
    {
        [SerializeField, ReadOnly, Range(0, 24)] private float timeOfDay = 10f;
        [SerializeField] private float dayStartTime = 8f;
        [SerializeField] private float nightStartTime = 22f;
        [SerializeField] private bool changeTimeAutomatically;

        private TimeOfDay currentTimeOfDay;

        public event Action<float> TimeChangedEvent;
        public event Action ChangedToDayEvent;
        public event Action ChangedToNightEvent;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            CheckTimeOfDay();
        }

        private void Update()
        {
            if (!changeTimeAutomatically)
            {
                return;
            }

            timeOfDay += UnityEngine.Time.deltaTime;

            OnTimeChanged();
        }

        private void OnTimeChanged()
        {
            timeOfDay %= 24;

            CheckTimeOfDay();

            TimeChangedEvent?.Invoke(timeOfDay);
        }

        private void SetTime(float value)
        {
            timeOfDay = value;
            OnTimeChanged();
        }

        private void CheckTimeOfDay()
        {
            if (timeOfDay >= dayStartTime && timeOfDay < nightStartTime)
            {
                if (currentTimeOfDay == TimeOfDay.Day)
                {
                    return;
                }

                currentTimeOfDay = TimeOfDay.Day;

                ChangedToDayEvent?.Invoke();
            }
            else
            {
                if (currentTimeOfDay == TimeOfDay.Night)
                {
                    return;
                }

                currentTimeOfDay = TimeOfDay.Night;

                ChangedToNightEvent?.Invoke();
            }
        }

        public void ToggleTimeOfDay(float duration, Action callback)
        {
            float targetTimeOfDay = currentTimeOfDay == TimeOfDay.Day ? nightStartTime : dayStartTime;

            DOVirtual.Float(timeOfDay, targetTimeOfDay, duration, SetTime).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }
}
