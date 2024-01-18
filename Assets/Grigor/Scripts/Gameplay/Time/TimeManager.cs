using System;
using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using Grigor.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Time
{
    [Injectable]
    public class TimeManager : MonoBehaviour
    {
        [SerializeField, Wrap(0, 24), HorizontalGroup("Time"), HideLabel] private int hours = 0;
        [ShowInInspector, HorizontalGroup("Time", Width = 0.01f), HideLabel, DisplayAsString] private string separator = ":";
        [SerializeField, Wrap(0, 60), HorizontalGroup("Time"), HideLabel] private int minutes = 0;
        [ShowInInspector, HorizontalGroup("Time", Width = 0.01f), HideLabel, DisplayAsString] private string secondSeparator = ":";
        [SerializeField, Wrap(0, 60), HorizontalGroup("Time"), HideLabel] private float seconds = 0f;
        [SerializeField, ReadOnly] private int days;

        private TimeOfDay currentTimeOfDay;
        private bool canEndDay;
        private readonly int totalDayMinutes = 60 * 24;
        private SceneConfig sceneConfig;
        private GameConfig gameConfig;

        public int TotalDayMinutes => totalDayMinutes;
        public int Days => days;

        public event Action<int, int> TimeChangedEvent;
        public event Action ChangedToDayEvent;
        public event Action ChangedToNightEvent;
        public event Action NewDayEvent;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            sceneConfig = SceneConfig.Instance;
            gameConfig = GameConfig.Instance;
        }

        private void Update()
        {
            if (!sceneConfig.ChangeTimeAutomatically)
            {
                return;
            }

            seconds += UnityEngine.Time.deltaTime * sceneConfig.TimeMultiplier;

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

                days++;

                NewDayEvent?.Invoke();

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
            if (hours >= sceneConfig.DayStartHour && hours < sceneConfig.NightStartHour)
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
            hours = SceneConfig.Instance.StartHour;

            if (hours >= sceneConfig.DayStartHour && hours < sceneConfig.NightStartHour)
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

            canEndDay = true;

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
            int targetTimeOfDay = currentTimeOfDay == TimeOfDay.Day ? sceneConfig.NightStartHour : sceneConfig.DayStartHour;

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
            days++;

            CheckStartTimeOfDay();

            OnTimeChanged();
        }

        public void PassTime(int minutesToPass, int hoursToPass)
        {
            int newMinutes = minutesToPass + hoursToPass * 60;

            int previousValue = 0;

            DOVirtual.Int(minutes, minutes + newMinutes, gameConfig.TimePassTweenDuration, value =>
            {
                if (previousValue == 0)
                {
                    previousValue = minutes;
                }

                if (value == previousValue)
                {
                    return;
                }

                minutes += value - previousValue;

                previousValue = value;

                OnTimeChanged();
            });
        }

        public void SetTimeToNight()
        {
            SetTimeToHour(sceneConfig.NightStartHour);
        }

        public void SetTimeToDay()
        {
            SetTimeToHour(sceneConfig.DayStartHour);
        }

        public void OnDayStarted()
        {
            canEndDay = true;

            SetTimeToDay();
        }

        public bool TryEndDay()
        {
            if (!canEndDay)
            {
                return false;
            }

            NewDayEvent?.Invoke();

            canEndDay = false;

            if (currentTimeOfDay == TimeOfDay.Day)
            {
                SetTimeToNight();
            }

            return true;
        }

        public float GetCurrentDayPercentage()
        {
            float currentPassedMinutes = hours * 60 + minutes;

            return currentPassedMinutes / totalDayMinutes;
        }
    }
}
