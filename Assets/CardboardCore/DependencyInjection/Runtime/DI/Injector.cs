using System.Reflection;
using CardboardCore.Utilities;

namespace CardboardCore.DI
{
    /// <summary>
    /// Static class to kick off any object to Inject into or Dump from given fields
    /// </summary>
    public static class Injector
    {
        public static void Inject(object @object)
        {
            FieldInfo[] fieldInfos = Reflection.GetFieldsWithAttribute<InjectAttribute>(@object.GetType());

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                (InjectionLayer, InjectableAttribute) injectionLayer = InjectionLayerManager.GetInjectionLayer(fieldInfos[i]);
                injectionLayer.Item1.InjectIntoField(fieldInfos[i], injectionLayer.Item2, @object);
            }
        }

        public static void Release(object @object)
        {
            FieldInfo[] fieldInfos = Reflection.GetFieldsWithAttribute<InjectAttribute>(@object.GetType());

            for (int i = 0; i < fieldInfos.Length; i++)
            {
                (InjectionLayer, InjectableAttribute) injectionLayer = InjectionLayerManager.GetInjectionLayer(fieldInfos[i]);
                injectionLayer.Item1.ReleaseDependencies(@object);
            }
        }
    }
}
