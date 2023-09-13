using System;

namespace CardboardCore.DI
{
    /// <summary>
    /// Add this attribute to a class to make it injectable. It will be a singleton by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectableAttribute : Attribute
    {
        /// <summary>
        /// The InjectionLayer this class is added to
        /// </summary>
        /// <returns></returns>
        public Type Layer { get; set; } = typeof(InjectionLayer);

        /// <summary>
        /// Is this class supposed to be approached as a singleton?
        /// </summary>
        /// <value></value>
        public bool Singleton { get; set; } = true;

        /// <summary>
        /// If the DI system will clean this object once no more references are found
        /// </summary>
        /// <value></value>
        public bool ClearAutomatically { get; set; } = false;
    }
}
