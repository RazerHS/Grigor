namespace CardboardCore.DI.Interfaces
{
    public interface DIInitializable
    {
        /// <summary>
        /// Called automatically the very first time an object is injected
        /// </summary>
        void Initialize();
    }
}
