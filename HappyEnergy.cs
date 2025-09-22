using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;
using Il2CppSystem;

namespace HappyEnergy;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<float> ProbMult;
    private static ConfigEntry<bool> GainToggle;
    private static ConfigEntry<float> GainMult;
    private static ConfigEntry<int> GainFlat;

    public override void Load()
    {
        ProbMult = Config.Bind("General", "Probability Multiplier", 1f, "Set number to multiply the probability of getting Happy Energy from various activities.\nSet value to 100 to make all activities guarantee Happy Energy.");
        GainToggle = Config.Bind("General", "Happy Energy Gain Toggle",true, "For Happy Energy gain, set this to \"true\" to use multipliers and \"false\" to use flat numbers.");
        GainMult = Config.Bind("General","Happy Energy Multiplier",1f,"Multiplies Happy Energy gains by the amount entered. Happy Energy Gain Toggle must be set to \"true\".");
        GainFlat = Config.Bind("General","Happy Energy Flat Gain",0,"Sets the amount of Happy Energy gained per action to the number entered. Happy Energy Gain Toggle must be set to \"false\".");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(HappyEnergyTweaks));
    }
    
    private class HappyEnergyTweaks
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void BiggerBag()
        {
            if (ProbMult.Value == 100f)
            {
                foreach (var happiness in MasterDataManager.Instance.HappyPointMaster.list)
                {
                    happiness.Probability = 100;
                }
            }
            else
            {
                foreach (var happiness in MasterDataManager.Instance.HappyPointMaster.list)
                {
                    happiness.Probability = (int)Math.Min(happiness.Probability * ProbMult.Value, 100);
                }
            }
            
            if (!GainToggle.Value)
            {
                foreach (var avarice in MasterDataManager.Instance.HappyPointMaster.list)
                {
                    avarice.AddPoint = (uint)GainFlat.Value;
                }
            }
            else
            {
                foreach (var avarice in MasterDataManager.Instance.HappyPointMaster.list)
                {
                    avarice.AddPoint = (uint)Math.Round(avarice.AddPoint * GainMult.Value);
                }
            }
        }
    }
}