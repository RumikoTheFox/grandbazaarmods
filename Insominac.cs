using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;

namespace Insominac;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<int> RecoveryRate;

    public override void Load()
    {
        RecoveryRate = Config.Bind("General", "Stamina Gain", 2000, "The stamina gained per hour of sleep. 2000 will heal you to full after an hour, even if you've eaten all Power Berries.\nIf you are using a mod that increases your max stamina past 2000, you can edit this value to match.");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Insomnia));
    }

    private class Insomnia
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void UpLate()
        {
            foreach (var wake in MasterDataManager.Instance.PlayerRecoveryMaster.list)
            {
                wake.WakeUpTime = "6:00";
                wake.PerHourRate = RecoveryRate.Value;
            }
        }
    }
}