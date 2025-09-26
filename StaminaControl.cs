using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;

namespace StaminaControl;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<int> BerryNumber;
    private static ConfigEntry<float> BerryValue;

    public override void Load()
    {
        BerryNumber = Config.Bind("General", "Number of Berries", 10, "Set to the number of berries you want to be required for max stamina. Game Default: 10");
        BerryValue = Config.Bind("General", "Stamina from Berries", 100f, "Set to how much stamina you want to gain from a single berry. Game Default: 100");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(StaminaSettings));
    }

    private class StaminaSettings
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void StamTime()
        {
            GameController.Instance.PlayerSetting.PlayerMaxHP = BerryNumber.Value * BerryValue.Value;
            GameController.Instance.PlayerSetting.MisteriousWaterValue = BerryValue.Value;
        }
    }
}
