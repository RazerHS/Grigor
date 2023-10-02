using Grigor.Data.Credentials;

namespace Grigor.Overworld.Clues
{
    public interface IClueListener
    {
        public void OnClueFound(CredentialType credentialType);
    }
}
