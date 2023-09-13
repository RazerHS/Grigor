using CardboardCore.DI;
using UnityEngine;

namespace Grigor.UI
{
    public abstract class UIElement : MonoBehaviour
    {
        internal void Show()
        {
            Injector.Inject(this);
            this.gameObject.SetActive(true);
            OnShow();
        }

        internal void Hide()
        {
            OnHide();
            Injector.Release(this);
            this.gameObject.SetActive(false);
        }

        protected abstract void OnShow();
        protected abstract void OnHide();
    }
}
