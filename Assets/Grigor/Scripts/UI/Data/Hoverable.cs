using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private bool changeSprite;
        [SerializeField] private bool changeColor;
        [SerializeField, ShowIf(nameof(changeSprite))] private Sprite hoverSprite;
        [SerializeField, ShowIf(nameof(changeColor))] private Color hoverColor;

        private Color defaultColor;
        private Sprite defaultSprite;
        private bool enabled = true;

        private void Awake()
        {
            defaultColor = image.color;
            defaultSprite = image.sprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!enabled)
            {
                return;
            }

            if (changeColor)
            {
                image.color = hoverColor;
            }

            if (changeSprite)
            {
                image.sprite = hoverSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!enabled)
            {
                return;
            }

            if (changeColor)
            {
                image.color = defaultColor;
            }

            if (changeSprite)
            {
                image.sprite = defaultSprite;
            }
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}
