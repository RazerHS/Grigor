using Grigor.Data.Clues;
using Grigor.Utils;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidencePicture : EvidenceNote
    {
        [SerializeField] private MeshRenderer pictureRenderer;

        public override void InitializeContents(EvidenceBoardNote evidenceBoardNote)
        {
            // NOTE: sharedMaterial can be used in the editor without causing scene material leaks, but this will change the
            // material asset itself, which changes the texture for every picture on the board.
            if (Application.isPlaying)
            {
                pictureRenderer.material.mainTexture = evidenceBoardNote.ClueData.EvidenceSprite.texture;
            }

            Vector3 scale = Helper.GetScaleBasedOnTextureSize(evidenceBoardNote.ClueData.TextureSize.x, evidenceBoardNote.ClueData.TextureSize.y, evidenceBoardNote.ClueData.UpscaleFactor);
            float aspect = evidenceBoardNote.ClueData.TextureSize.x / evidenceBoardNote.ClueData.TextureSize.y;

            evidenceBoardNote.ScaleContents(scale, evidenceBoardNote.ClueData.UpscaleFactor, aspect);
        }
    }
}
