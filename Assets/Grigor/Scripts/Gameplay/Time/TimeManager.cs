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

        private bool isDayTime;

        public event Action<float> TimeChangedEvent;
        public event Action ChangedToDayTimeEvent;
        public event Action ChangedToNightTimeEvent;

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
                isDayTime = true;

                ChangedToDayTimeEvent?.Invoke();
            }
            else
            {
                isDayTime = false;

                ChangedToNightTimeEvent?.Invoke();
            }
        }

        public void ToggleTimeOfDay(float duration, Action callback)
        {
            float targetTimeOfDay = isDayTime ? nightStartTime : dayStartTime;

            DOVirtual.Float(timeOfDay, targetTimeOfDay, duration, SetTime).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }
}
