using System;

namespace CardboardCore.DI
{
    /// <summary>
    /// Add this attribute to a field to inject an instance into given field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {

    }
}
