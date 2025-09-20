using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono;
using HarmonyLib;

namespace BathTweaks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<int> BaseHeal;
    private static ConfigEntry<int> UpgradedHeal;
    private static ConfigEntry<int> BathCooldown;
    private static ConfigEntry<float> TimeElapsed;
    private static ConfigEntry<float> FadeToBlack;

    public override void Load()
    {
        BaseHeal = Config.Bind("General", "Base Healing", 800, "Amount of stamina healed per bath. Max stamina for player is 1000 plus 100 for each power berry eaten. Game Default: 500");
        UpgradedHeal = Config.Bind("General", "Upgraded Healing", 1750, "Amount of stamina healed per upgraded bath. Max stamina for player is 1000 plus 100 for each power berry eaten. Game Default: 1000");
        BathCooldown = Config.Bind("General", "Bath Cooldown", 1, "Cooldown in hours before the bath can be used again. Cannot have decimal places. Game Default: 8");
        TimeElapsed = Config.Bind("General", "Time Elapsed", 0.5f, "Time passed when taking a bath in hours. 0.5 = 30 minutes. Game Default: 0.5");
        FadeToBlack = Config.Bind("General", "FadeToBlack", 0f, "Time in seconds it takes to take a bath. Lowering this value makes the fade to black shorter and gives back control faster. Game Default: 6");
        
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(BathData));
    }

    private class BathData
    {
        [HarmonyPatch(typeof(BathManager), "Use")]
        [HarmonyPrefix]
        private static void Prefix(UserInfo __instance)
        {
            // Grab the BathSetting instance from SettingAssetManager
            var bSetting = ManagedSingleton<SettingAssetManager>.Instance.BathSetting;
            
            // Edit the BathSettings properties as you need
            bSetting.HealValueRank1 = BaseHeal.Value;
            bSetting.HealValueRank2 = UpgradedHeal.Value;
            bSetting.UseableHourSpan = BathCooldown.Value;
            bSetting.ConsumeTime = TimeElapsed.Value;
            bSetting.FadeWaitTime = FadeToBlack.Value;
        }
        
        [HarmonyPatch(typeof(BathManager), "Use")]
        [HarmonyPostfix]
        private static void Postfix(UserInfo __instance)
        {
            // Grab the BathSetting instance from SettingAssetManager
            var bSetting = ManagedSingleton<SettingAssetManager>.Instance.BathSetting;
            
            // Edit the BathSettings properties as you need
            Log.LogInfo("Heal1: " + bSetting.HealValueRank1 +
                        "\tHeal2: " + bSetting.HealValueRank2 +
                        "\tHourSpan: " + bSetting.UseableHourSpan +
                        "\tConsumeTime: " + bSetting.ConsumeTime +
                        "\tFadeWait: " + bSetting.FadeWaitTime);
        }
    }
}