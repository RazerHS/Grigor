using CardboardCore.DI;
using CardboardCore.DI.Interfaces;
using UnityEngine;

// This is a MonoBehaviour component like any other. But due to the Injectable attribute it's accessible everywhere!
// This is not to replace SerializeField, and it is recommended to keep using SerializeField if you're working
// object oriented without having the need for a commanding class (like "managers")
[Injectable(ClearAutomatically = true)]
public class Foo : MonoBehaviour, DIInitializable, DIDisposable
{
    public string Hello => "hello!";
    public string GoodBye => "goodbye!";

    public void Initialize()
    {

    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
