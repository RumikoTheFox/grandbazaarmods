using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono;
using HarmonyLib;

namespace StaminaWarnings
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        private static ConfigEntry<int> FirstWarning;
        private static ConfigEntry<int> SecondWarning;

        public override void Load()
        {
            FirstWarning = Config.Bind("General", "First Warning", 25, "Percentage Stamina for first warning.");
            SecondWarning = Config.Bind("General", "Second Warning", 5, "Percentage Stamina for second warning.");
            
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(WarningsClass));
        }
        
        private class WarningsClass
        {
            [HarmonyPatch(typeof(DateManager), "OnStartGame")]
            [HarmonyPostfix]
            public static void Alerts()
            {
                GameController.Instance.PlayerSetting.PlayerTiredPercent = FirstWarning.Value;
                GameController.Instance.PlayerSetting.PlayerVeryTiredPercent = SecondWarning.Value;
            }
        }
    }
}
