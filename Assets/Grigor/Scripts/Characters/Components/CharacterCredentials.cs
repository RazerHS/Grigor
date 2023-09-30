using System;
using Grigor.Data.Credentials;
using Sirenix.OdinInspector;

namespace Grigor.Characters.Components
{
    [Serializable]
    public class CharacterCredentials : CharacterComponent
    {
        [ShowInInspector, ReadOnly] private CredentialWallet credentialWallet;

        protected override void OnInitialized()
        {
            credentialWallet.InitializeWalletForCharacter(Owner);
        }
    }
}
