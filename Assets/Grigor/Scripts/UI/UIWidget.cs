namespace Grigor.UI
{
    public abstract class UIWidget : UIElement
    {
        protected override abstract void OnShow();
        protected override abstract void OnHide();
    }
}
