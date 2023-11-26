using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Utils;
using PlasticGui;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidencePicture : EvidenceNote
    {
        [SerializeField] private MeshRenderer pictureRenderer;

        private Material materialInstance;

        public override void OnInitializeContents(EvidenceBoardNote evidenceBoardNote)
        {
            if (evidenceBoardNote.ClueData.EvidenceSprite == null)
            {
                throw Log.Exception($"{evidenceBoardNote.ClueData.ClueHeading} has no sprite set!");
            }

            //creating a temporary instance of the shared material to avoid material leaks when trying to
            //change the texture for each note in the editor
            materialInstance = new Material(pictureRenderer.sharedMaterial)
            {
                mainTexture = evidenceBoardNote.ClueData.EvidenceSprite.texture
            };

            pictureRenderer.sharedMaterial = materialInstance;

            Vector3 scale = Helper.GetScaleBasedOnTextureSize(evidenceBoardNote.ClueData.TextureSize.x, evidenceBoardNote.ClueData.TextureSize.y, evidenceBoardNote.ClueData.UpscaleFactor);
            float aspect = evidenceBoardNote.ClueData.TextureSize.x / evidenceBoardNote.ClueData.TextureSize.y;

            evidenceBoardNote.ScaleContents(scale, evidenceBoardNote.ClueData.UpscaleFactor, aspect);
        }
    }
}
