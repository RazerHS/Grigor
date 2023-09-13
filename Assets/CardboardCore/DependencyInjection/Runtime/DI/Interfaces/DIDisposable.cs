namespace CardboardCore.DI.Interfaces
{
    public interface DIDisposable
    {
        /// <summary>
        /// Called automatically the last time an object is released
        /// </summary>
        void Dispose();
    }
}
