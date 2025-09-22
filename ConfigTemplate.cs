using BepInEx.Configuration;

namespace YourNamespace;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<type> VariableName;

    public override void Load()
    {
        VariableName = Config.Bind("Category Name", "Short Name for Config Setting", DefaultValue, "Longer description of setting.");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
    
    private class ClassName
    {
        [HarmonyPatchStuff]
        private static void MethodName()
        {
            ValueYouWantToChange = VariableName.Value;
		}	
    }
}
