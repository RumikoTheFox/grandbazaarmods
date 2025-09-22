using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System.Collections.Generic;
using BokuMono;
using BokuMono.Data;
using HarmonyLib;

namespace BugsDontRun;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(BugBehavior));
    }

    private class BugBehavior
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void BugLure()
        {
            foreach (var bugrun in MasterDataManager.Instance.WildAnimalMaster.list)
            {
                bugrun.IsEscape = false;
            }
        }
    }
}