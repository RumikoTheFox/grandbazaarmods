using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono.Data;
using HarmonyLib;

namespace FreshnessRework;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(FreshnessBoost));
    }

    private class FreshnessBoost
    {
        [HarmonyPatch(typeof(ItemData), nameof(ItemData.ReduceFreshness))]
        [HarmonyPostfix]

        public static void Postfix(ItemData __instance, int __0)
        {
            if (__instance.HasFreshness)
            {
                __instance.FreshnessValue = 100;
            }
        }
    }
}