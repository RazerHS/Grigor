#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;

namespace CardboardCore.DI
{
    [InitializeOnLoad]
    public class DefineSymbolManager
    {
        static DefineSymbolManager()
        {            
            // TODO: I'd prefer to update define symbols before actually removing the package, but listening to "Events.registeringPackages" does not seem to work 
            Events.registeringPackages += EventsOnregisteringPackages;
            
            // Note: This is only here because the above event is never triggered
            Events.registeredPackages += EventsOnregisteringPackages;
            
            // Register required define symbol for other packages to rely on
            string symbols = GetScriptingDefineSymbols();
            if (symbols.Contains("CC_DI;"))
            {
                return;
            }

            symbols += "CC_DI;";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        }

        private static void EventsOnregisteringPackages(PackageRegistrationEventArgs obj)
        { if (obj.removed.Any(t => t.name == "com.cardboardcore.di"))
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                symbols = symbols.Replace("CC_DI", "");

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }

        private static string GetScriptingDefineSymbols()
        {
            // Get and store current scripting define symbols
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!string.IsNullOrEmpty(symbols))
            {
                symbols += ";";
            }

            return symbols;
        }
    }
}
#endif