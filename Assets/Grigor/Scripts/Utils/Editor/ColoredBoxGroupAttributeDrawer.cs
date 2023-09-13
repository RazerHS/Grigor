using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using Grigor.Utils.Attributes;

namespace Grigor.Utils.Editor
{
    public class ColoredBoxGroupAttributeDrawer : OdinGroupDrawer<ColoredBoxGroupAttribute>
    {
        private LocalPersistentContext<bool> isExpanded;

        protected override void Initialize()
        {
            this.isExpanded = this.GetPersistentValue<bool>("ColoredFoldoutGroupAttributeDrawer.isExpanded", GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            string headerLabel = Attribute.LabelText;

            GUIHelper.PushColor(new Color(this.Attribute.R, this.Attribute.G, this.Attribute.B, this.Attribute.A));
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();

            GUIHelper.PopColor();

            if (Attribute.Fold)
            {
                this.isExpanded.Value = SirenixEditorGUI.Foldout(this.isExpanded.Value, label);
            }
            else
            {
                SirenixEditorGUI.Title(headerLabel, null, TextAlignment.Left, false, false);
            }

            SirenixEditorGUI.EndBoxHeader();

            if (Attribute.Fold)
            {
                if (SirenixEditorGUI.BeginFadeGroup(this, this.isExpanded.Value))
                {
                    DrawChildren();
                }

                SirenixEditorGUI.EndFadeGroup();
            }
            else
            {
                DrawChildren();
            }

            SirenixEditorGUI.EndBox();
        }

        private void DrawChildren()
        {
            for (int i = 0; i < this.Property.Children.Count; i++)
            {
                this.Property.Children[i].Draw();
            }
        }
    }
}
