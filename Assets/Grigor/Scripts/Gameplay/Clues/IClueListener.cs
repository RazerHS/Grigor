using Grigor.Data.Credentials;

namespace Grigor.Gameplay.Clues
{
    public interface IClueListener
    {
        public void OnClueFound(CredentialType credentialType);

        public void RegisterClueListener();
    }
}
