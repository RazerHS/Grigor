using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    [Injectable]
    public class TimeManager : MonoBehaviour
    {
        [SerializeField, ReadOnly, Range(0, 60)] private float seconds = 0f;
        [SerializeField, ReadOnly, Range(0, 60)] private int minutes = 0;
        [SerializeField, ReadOnly, Range(0, 24)] private int hours = 0;
        [SerializeField, ReadOnly] private int daysPassed;
        [SerializeField] private int dayStartHour = 8;
        [SerializeField] private int nightStartHour = 22;
        [SerializeField] private bool changeTimeAutomatically;
        [SerializeField, ShowIf(nameof(changeTimeAutomatically))] private float timeMultiplier;

        private TimeOfDay currentTimeOfDay;

        public event Action<int, int> TimeChangedEvent;
        public event Action ChangedToDayEvent;
        public event Action ChangedToNightEvent;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!changeTimeAutomatically)
            {
                return;
            }

            seconds += UnityEngine.Time.deltaTime * timeMultiplier;

            OnTimeChanged();
        }

        private void OnTimeChanged()
        {
            ParseSeconds();
            ParseMinutes();
            ParseHours();

            CheckTimeOfDay();

            TimeChangedEvent?.Invoke(minutes, hours);
        }

        private void ParseSeconds()
        {
            if (seconds < 60)
            {
                return;
            }

            seconds = 0;
            minutes++;
        }

        private void ParseMinutes()
        {
            while (true)
            {
                if (minutes < 60)
                {
                    return;
                }

                hours++;

                minutes -= 60;

                if (minutes >= 60)
                {
                    continue;
                }

                break;
            }
        }

        private void ParseHours()
        {
            while (true)
            {
                if (hours < 24)
                {
                    return;
                }

                daysPassed++;

                hours -= 24;

                if (hours >= 24)
                {
                    continue;
                }

                break;
            }
        }

        private void CheckTimeOfDay()
        {
            if (hours >= dayStartHour && hours < nightStartHour)
            {
                if (currentTimeOfDay == TimeOfDay.Day)
                {
                    return;
                }

                OnChangedToDay();
            }
            else
            {
                if (currentTimeOfDay == TimeOfDay.Night)
                {
                    return;
                }

                OnChangedToNight();
            }
        }

        private void CheckStartTimeOfDay()
        {
            hours = dayStartHour;

            if (hours >= dayStartHour && hours < nightStartHour)
            {
               OnChangedToDay();
            }
            else
            {
               OnChangedToNight();
            }

            OnTimeChanged();
        }

        private void OnChangedToDay()
        {
            currentTimeOfDay = TimeOfDay.Day;

            Log.Write("Day started!");

            ChangedToDayEvent?.Invoke();
        }

        private void OnChangedToNight()
        {
            currentTimeOfDay = TimeOfDay.Night;

            Log.Write("Night started!");

            ChangedToNightEvent?.Invoke();
        }

        public void ToggleTimeOfDay(float duration, Action callback)
        {
            int targetTimeOfDay = currentTimeOfDay == TimeOfDay.Day ? nightStartHour : dayStartHour;

            DOVirtual.Int(hours, targetTimeOfDay, duration, SetTimeToHour).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }

        private void SetTimeToHour(int value)
        {
            hours = value;
            minutes = 0;
            seconds = 0;

            OnTimeChanged();
        }

        public void StartTime()
        {
            CheckStartTimeOfDay();

            OnTimeChanged();
        }

        public void PassTime(int minutes, int hours)
        {
            this.minutes += minutes;
            this.hours += hours;

            Log.Write($"Time passed! New time: <b>{this.hours}:{this.minutes}</b>");

            OnTimeChanged();
        }

        public void SetTimeToNight()
        {
            SetTimeToHour(nightStartHour);
        }

        public void SetTimeToDay()
        {
            SetTimeToHour(dayStartHour);
        }
    }
}
