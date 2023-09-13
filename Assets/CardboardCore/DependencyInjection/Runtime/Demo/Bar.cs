using CardboardCore.DI;
using CardboardCore.Utilities;

// Extending `CardboardCoreBehaviour` instead of `MonoBehaviour`. To have some out of the box support for Injection.
// This is not mandatory, but might make life easier when dealing with Injection in MonoBehaviours.
public class Bar : CardboardCoreBehaviour
{
    // All fields in this class with the `Inject` attribute will be injected at Unity's `OnEnable`, and Released again
    // at Unity's `OnDisable`.
    protected override InjectTiming MyInjectTiming => InjectTiming.OnEnable;

    // The injected class. This can either be a pure C# class or a `MonoBehaviour`.
    // If it's a `MonoBehaviour`, it's wise to always have just one or zero of such objects in your scene.
    // With more than one object, the Injector doesn't know which one to pick and will inject the first object it will
    // find (which could cause issues for you).
    // With zero objects in your scene, it will create an object and attach the `MonoBehaviour` (in this case `Foo`)
    // and apply DontDestroyOnLoad.
    [Inject] private Foo foo;

    // This is an abstract method in `CardboardCoreBehaviour` (of which this class is derived from).
    // Based on the above selected `InjectTiming`, this method will be called accordingly.
    // Use this method for initialization, as you'll always know that every field in this class is now injected and
    // can no longer be `null`.
    protected override void OnInjected()
    {
        // Logging Foo.Hello string. Play around in the editor during runtime with enabling/disabling this GameObject
        // and see what happens!
        Log.Write(foo.Hello);
    }

    // This is an abstract method in `CardboardCoreBehaviour` (of which this class is derived from).
    // Based on the above selected `InjectTiming`, this method will be called accordingly.
    // Use this method for cleanup, as you'll always know that every field which was injected will be released
    // (and thus become `null`) right after this method has called.
    protected override void OnReleased()
    {
        // Logging Foo.Goodbye string. Play around in the editor during runtime with enabling/disabling this GameObject
        // and see what happens!
        Log.Write(foo.GoodBye);
    }
}
