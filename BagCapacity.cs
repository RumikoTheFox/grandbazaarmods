using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;
using Il2CppSystem;

namespace BagCapacity;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<float> Multiplier;
    private static ConfigEntry<bool> BagSize;

    public override void Load()
    {
        Multiplier = Config.Bind("General", "Price Multiplier", 1f, "Edit to whichever multiplier you want for the gold cost for upgrading your bags. Set to 0 for free.\nSomething like 0.5 will lower the cost (50% in this case), where something like 3 will increase the cost (x3).");
        BagSize = Config.Bind("General", "Enable More Slots", true, "Set to true or false to allow or disallow the change to slot capacity.");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(CapacityIncrease));
    }
    
    private class CapacityIncrease
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void BiggerBag()
        {
            foreach (var chonk in MasterDataManager.Instance.BagMaster.list)
            {
                if (BagSize.Value)
                {
                    chonk.Capacity = 32;
                }
            }

            var stonks0 = MasterDataManager.Instance.ExpansionMaster.list[0];
            var stonks1 = MasterDataManager.Instance.ExpansionMaster.list[1];
            stonks0.Price = (int)Math.Round(stonks0.Price * Multiplier.Value);
            stonks1.Price = (int)Math.Round(stonks1.Price * Multiplier.Value);
        }
    }
}