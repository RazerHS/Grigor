using System;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class ExposedProperty
    {
        public string PropertyName = "New String";
        public string PropertyValue = "New Value";

        public static ExposedProperty CreateInstance()
        {
            return new ExposedProperty();
        }
    }
}
