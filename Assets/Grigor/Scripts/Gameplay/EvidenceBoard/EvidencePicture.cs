using CardboardCore.Utilities;
using Grigor.Utils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Grigor.Gameplay.EvidenceBoard
{
    public class EvidencePicture : EvidenceNote
    {
        [SerializeField] private MeshRenderer pictureRenderer;
        [SerializeField] private Material defaultMaterial;

        [SerializeField] private Material materialInstance;

        public override void OnInitializeContents(EvidenceBoardNote evidenceBoardNote)
        {
            if (evidenceBoardNote.ClueData.EvidenceSprite == null)
            {
                throw Log.Exception($"{evidenceBoardNote.ClueData.ClueHeading} has no sprite set!");
            }

            if (pictureRenderer.sharedMaterial == null)
            {
                pictureRenderer.sharedMaterial = defaultMaterial;
            }

            //creating a temporary instance of the shared material to avoid material leaks when trying to
            //change the texture for each note in the editor
            materialInstance = new Material(pictureRenderer.sharedMaterial)
            {
                mainTexture = evidenceBoardNote.ClueData.EvidenceSprite.texture
            };

            pictureRenderer.sharedMaterial = materialInstance;

            Vector3 scale = Helper.GetScaleBasedOnTextureSize(evidenceBoardNote.ClueData.TextureSize.x, evidenceBoardNote.ClueData.TextureSize.y, evidenceBoardNote.ClueData.UpscaleFactor);

            if (scale == Vector3.zero)
            {
                throw Log.Exception("Scale must not be zero!");
            }

            float aspect = evidenceBoardNote.ClueData.TextureSize.x / evidenceBoardNote.ClueData.TextureSize.y;

            evidenceBoardNote.ScaleContents(scale, evidenceBoardNote.ClueData.UpscaleFactor, aspect);
        }
    }
}
