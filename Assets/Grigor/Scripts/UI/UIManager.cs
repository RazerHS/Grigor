using CardboardCore.DI;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CardboardCore.Utilities;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Grigor.UI
{
    [Injectable]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private List<UIScreen> screens;
        [SerializeField] private List<UIWidget> widgets;

        private UIScreen currentScreen;

        [Button]
        private void GetAllElements()
        {
            screens = GetComponentsInChildren<UIScreen>(true).ToList();
            widgets = GetComponentsInChildren<UIWidget>(true).ToList();
        }

        private void Awake()
        {
            screens.ForEach(screen => screen.Hide());
            widgets.ForEach(widget => widget.Hide());
        }

        public T ShowScreen<T>() where T : UIScreen
        {
            T newScreen = screens.FirstOrDefault(screen => screen.GetType() == typeof(T)) as T;

            if (newScreen == null)
            {
                throw Log.Exception($"Cannot find UIScreen of type {typeof(T).Name}.");
            }

            if (currentScreen != null)
            {
                currentScreen.Hide();
            }

            newScreen.Show();

            currentScreen = newScreen;

            return newScreen;
        }

        public T ShowWidget<T>() where T : UIWidget
        {
            T newWidget = widgets.FirstOrDefault(widget => widget.GetType() == typeof(T)) as T;

            if (newWidget == null)
            {
                throw Log.Exception($"Cannot find UIWidget of type {typeof(T).Name}.");
            }

            newWidget.Show();

            return newWidget;
        }

        public void HideWidget<T>() where T : UIWidget
        {
            UIWidget widget = widgets.FirstOrDefault(widget => widget.GetType() == typeof(T));

            if (widget == null)
            {
                return;
            }

            widget.Hide();
        }

        public T GetScreen<T>() where T : UIScreen
        {
            T screen = screens.FirstOrDefault(screen => screen.GetType() == typeof(T)) as T;

            if (screen == null)
            {
                throw Log.Exception($"Cannot find UIScreen of type {typeof(T).Name}.");
            }

            return screen;
        }

        public T GetWidget<T>() where T : UIWidget
        {
            T widget = widgets.FirstOrDefault(widget => widget.GetType() == typeof(T)) as T;

            if (widget == null)
            {
                throw Log.Exception($"Cannot find UIWidget of type {typeof(T).Name}.");
            }

            return widget;
        }
    }
}
