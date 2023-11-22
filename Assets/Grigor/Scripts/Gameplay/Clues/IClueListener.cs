using Grigor.Data.Clues;
using Grigor.Data.Credentials;

namespace Grigor.Gameplay.Clues
{
    public interface IClueListener
    {
        public void OnClueFound(ClueData clueData);

        public void RegisterClueListener();
    }
}
