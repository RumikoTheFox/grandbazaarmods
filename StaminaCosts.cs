using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono.Data;
using HarmonyLib;

namespace StaminaCosts;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<float> BrushCost;
    private static ConfigEntry<float> ClippingCost;
    private static ConfigEntry<float> MilkingCost;
    private static ConfigEntry<float> FishingCost;
    private static ConfigEntry<float> HoeCost;
    private static ConfigEntry<float> HoeJCost;
    private static ConfigEntry<float> HoeDJCost;
    private static ConfigEntry<float> WaterCost;
    private static ConfigEntry<float> WaterJCost;
    private static ConfigEntry<float> WaterDJCost;
    private static ConfigEntry<float> SickleCost;
    private static ConfigEntry<float> SickleJCost;
    private static ConfigEntry<float> SickleDJCost;
    private static ConfigEntry<float> HatchetCost;
    private static ConfigEntry<float> HatchetJCost;
    private static ConfigEntry<float> UHatchetCost;
    private static ConfigEntry<float> UHatchetJCost;
    private static ConfigEntry<float> CollectCost;
    private static ConfigEntry<float> InsectCost;
    private static ConfigEntry<float> MushroomCost;
    private static ConfigEntry<float> BeesCost;
    private static ConfigEntry<float> CultivateCost;
    private static ConfigEntry<float> CultivateJCost;
    private static ConfigEntry<float> CultivateDJCost;
    private static ConfigEntry<float> HarvestJCost;
    private static ConfigEntry<float> HarvestDJCost;
    private static ConfigEntry<float> FertilizeCost;
    private static ConfigEntry<float> FertilizeJCost;
    private static ConfigEntry<float> FertilizeDJCost;
    private static ConfigEntry<float> FallWaterCost;
    private static ConfigEntry<float> TeleportCost;
    private static ConfigEntry<float> TrainingCost;
    private static ConfigEntry<float> SnowballCost;
    private static ConfigEntry<float> LeavesCost;

    public override void Load()
    {
        BrushCost = Config.Bind("Tools", "Brush", 10f, "Cost of using the Brush. Default: 15");
        ClippingCost = Config.Bind("Tools", "Clipper", 10f, "Cost of using the Clippers. Default: 15");
        MilkingCost = Config.Bind("Tools", "Milker", 10f, "Cost of using the Milker. Default: 15");
        FishingCost = Config.Bind("Tools", "Fishing", 20f, "Cost of using the Fishing Rod. Default: 40");
        HoeCost = Config.Bind("Tools", "Hoe", 5f, "Cost of using the Hoe. Default: 5");
        HoeJCost = Config.Bind("Tools", "Hoe Jump", 15f, "Cost of using the Jump Hoe. Default: 20");
        HoeDJCost = Config.Bind("Tools", "Hoe Double Jump", 35f, "Cost of using the Double Jump Hoe. Default: 50");
        WaterCost = Config.Bind("Tools", "Watering Can", 5f, "Cost of using the Watering Can. Default: 10");
        WaterJCost = Config.Bind("Tools", "Watering Can Jump", 15f, "Cost of using the Jump Watering Can. Default: 30");
        WaterDJCost = Config.Bind("Tools", "Watering Can Double Jump", 35f,
            "Cost of using the Double Jump Watering Can. Default: 70");
        SickleCost = Config.Bind("Tools", "Sickle", 5f, "Cost of using the Sickle. Default: 5");
        SickleJCost = Config.Bind("Tools", "Sickle Jump", 15f, "Cost of using the Jump Sickle. Default: 15");
        SickleDJCost = Config.Bind("Tools", "Sickle Double Jump", 35f,
            "Cost of using the Double Jump Sickle. Default: 35");
        HatchetCost = Config.Bind("Tools", "Hatchet", 5f, "Cost of using the Hatchet. Default: 15");
        HatchetJCost = Config.Bind("Tools", "Hatchet Jump", 15f, "Cost of using the Jump Hatchet. Default: 60");
        UHatchetCost = Config.Bind("Tools", "Ultimate Hatchet", 5f, "Cost of using the Ultimate Hatchet. Default: 5");
        UHatchetJCost = Config.Bind("Tools", "Ultimate Hatchet Jump", 15f,
            "Cost of using the Jump Ultimate Hatchet. Default: 20");
        CollectCost = Config.Bind("Other", "Collecting", 0f, "Cost of foraging items. Default: 5");
        InsectCost = Config.Bind("Other", "Insect Pickup", 0f, "Cost of catching insects. Default: 20");
        MushroomCost = Config.Bind("Other", "Mushroom Log", 0f, "Cost of using the Mushroom Log. Default: 10");
        BeesCost = Config.Bind("Other", "Beehive", 0f, "Cost of using the Beehive. Default: 10");
        CultivateCost = Config.Bind("Other", "Sow Seeds", 0f, "Cost of sowing seeds. Default: 10");
        CultivateJCost = Config.Bind("Other", "Sow Seeds Jump", 10f, "Cost of sowing seeds while Jumping. Default: 30");
        CultivateDJCost = Config.Bind("Other", "Sow Seeds Double Jump", 25f,
            "Cost of sowing seeds while Double Jumping. Default: 70");
        HarvestJCost = Config.Bind("Other", "Harvest Jump", 10f, "Cost of harvesting while Jumping. Default: 20");
        HarvestDJCost = Config.Bind("Other", "Harvest Double Jump", 25f,
            "Cost of harvesting while Double Jumping. Default: 50");
        FertilizeCost = Config.Bind("Other", "Fertilize", 0f, "Cost of using Fertilizer. Default: 5");
        FertilizeJCost = Config.Bind("Other", "Fertilize Jump", 10f,
            "Cost of using Fertilizer while Jumping. Default: 20");
        FertilizeDJCost = Config.Bind("Other", "Fertilize Double Jump", 25f,
            "Cost of using Fertilizer while Double Jumping. Default: 50");
        FallWaterCost = Config.Bind("Other", "Falling in Water", 0f, "Cost of falling in water. Default: 100");
        TeleportCost = Config.Bind("Other", "Teleporting", 0f, "Cost of using the Travel Stone. Default: 100");
        TrainingCost = Config.Bind("Other", "Pet Training", 0f, "Cost of training your pets. Default: 20");
        SnowballCost = Config.Bind("Other", "Snowball", 0f, "Cost of throwing Snowballs? Default: 30");
        LeavesCost = Config.Bind("Other", "Fallen Leaves", 0f,
            "Cost of interacting with leaf piles in Autumn? Default: 30");

        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(StaminaCostClass));
    }

    private class StaminaCostClass
    {
        [HarmonyPatch(typeof(BokuMono.Data.PlayerHPMaster), "GetMasterDataByCategory")]
        [HarmonyPostfix]
        static void Postfix(BokuMono.Data.PlayerHPMaster __instance, BokuMono.Data.PlayerHPMasterData __result,
            BokuMono.Data.PlayerHPMaster.ActionCategory __0)
        {
            try
            {
                __result.HP = __0 switch
                {
                    PlayerHPMaster.ActionCategory.Brush => BrushCost.Value,
                    PlayerHPMaster.ActionCategory.Clipping => ClippingCost.Value,
                    PlayerHPMaster.ActionCategory.Milking => MilkingCost.Value,
                    PlayerHPMaster.ActionCategory.Fishing => FishingCost.Value,
                    PlayerHPMaster.ActionCategory.Sow => HoeCost.Value,
                    PlayerHPMaster.ActionCategory.JumpSow => HoeJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpSow => HoeDJCost.Value,
                    PlayerHPMaster.ActionCategory.Water => WaterCost.Value,
                    PlayerHPMaster.ActionCategory.JumpWater => WaterJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpWater => WaterDJCost.Value,
                    PlayerHPMaster.ActionCategory.Sickle => SickleCost.Value,
                    PlayerHPMaster.ActionCategory.JumpSickle => SickleJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpSickle => SickleDJCost.Value,
                    PlayerHPMaster.ActionCategory.Hatchet => HatchetCost.Value,
                    PlayerHPMaster.ActionCategory.JumpHatchet => HatchetJCost.Value,
                    PlayerHPMaster.ActionCategory.UltimateHatchet => UHatchetCost.Value,
                    PlayerHPMaster.ActionCategory.UltimateJumpHatchet => UHatchetJCost.Value,
                    PlayerHPMaster.ActionCategory.Collect => CollectCost.Value,
                    PlayerHPMaster.ActionCategory.CatchInsect => InsectCost.Value,
                    PlayerHPMaster.ActionCategory.Mushroom => MushroomCost.Value,
                    PlayerHPMaster.ActionCategory.Beekeeping => BeesCost.Value,
                    PlayerHPMaster.ActionCategory.Cultivate => CultivateCost.Value,
                    PlayerHPMaster.ActionCategory.JumpCultivate => CultivateJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpCultivate => CultivateDJCost.Value,
                    PlayerHPMaster.ActionCategory.JumpHarvest => HarvestJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpHarvest => HarvestDJCost.Value,
                    PlayerHPMaster.ActionCategory.Fertilize => FertilizeCost.Value,
                    PlayerHPMaster.ActionCategory.JumpFertilize => FertilizeJCost.Value,
                    PlayerHPMaster.ActionCategory.DoubleJumpFertilize => FertilizeDJCost.Value,
                    PlayerHPMaster.ActionCategory.FallWater => FallWaterCost.Value,
                    PlayerHPMaster.ActionCategory.FlyingStone => TeleportCost.Value,
                    PlayerHPMaster.ActionCategory.PetTraining => TrainingCost.Value,
                    PlayerHPMaster.ActionCategory.SnowBall => SnowballCost.Value,
                    PlayerHPMaster.ActionCategory.FallenLeaves => LeavesCost.Value,
                    _ => __result.HP,
                };
            }
            catch
            {
            }
        }
    }

}