using CardboardCore.DI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CardboardCore.Utilities;
using Sirenix.OdinInspector;

namespace Grigor.UI
{
    [Injectable]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIScreen> _screens;
        [SerializeField] private List<UIWidget> _widgets;

        private UIScreen _currentScreen;

        [Button]
        private void GetAllElements()
        {
            _screens = GetComponentsInChildren<UIScreen>().ToList();
            _widgets = GetComponentsInChildren<UIWidget>().ToList();
        }

        private void Awake()
        {
            _screens.ForEach(screen => screen.Hide());
            _widgets.ForEach(widget => widget.Hide());
        }

        public T ShowScreen<T>() where T : UIScreen
        {
            T newScreen = _screens.FirstOrDefault(screen => screen.GetType() == typeof(T)) as T;

            if (newScreen == null)
            {
                throw Log.Exception($"Cannot find UIScreen of type {typeof(T).Name}.");
            }

            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }

            newScreen.Show();
            _currentScreen = newScreen;

            return newScreen;
        }

        public T ShowWidget<T>() where T : UIWidget
        {
            T newWidget = _widgets.FirstOrDefault(widget => widget.GetType() == typeof(T)) as T;

            if (newWidget == null)
            {
                throw Log.Exception($"Cannot find UIWidget of type {typeof(T).Name}.");
            }

            newWidget.Show();
            return newWidget;
        }

        public void HideWidget<T>() where T : UIWidget
        {
            UIWidget widget = _widgets.FirstOrDefault(widget => widget.GetType() == typeof(T));

            if (widget == null)
            {
                return;
            }

            widget.Hide();
        }

        public T GetScreen<T>() where T : UIScreen
        {
            T screen = _screens.FirstOrDefault(screen => screen.GetType() == typeof(T)) as T;

            if (screen == null)
            {
                throw Log.Exception($"Cannot find UIScreen of type {typeof(T).Name}.");
            }

            return screen;
        }

        public T GetWidget<T>() where T : UIWidget
        {
            T widget = _widgets.FirstOrDefault(widget => widget.GetType() == typeof(T)) as T;

            if (widget == null)
            {
                throw Log.Exception($"Cannot find UIWidget of type {typeof(T).Name}.");
            }

            return widget;
        }
    }
}
