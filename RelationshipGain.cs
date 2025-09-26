using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;
using Il2CppSystem;

namespace RelationshipGain;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<string> GrowthType;
    private static ConfigEntry<int> Hearts1;
    private static ConfigEntry<int> Hearts2;
    private static ConfigEntry<int> Hearts3;
    private static ConfigEntry<int> Hearts4;
    private static ConfigEntry<int> Hearts5;
    private static ConfigEntry<int> Hearts6;
    private static ConfigEntry<int> Hearts7;
    private static ConfigEntry<int> Hearts8;
    private static ConfigEntry<int> Hearts9;
    private static ConfigEntry<int> Hearts10;

    private static ConfigEntry<string> WeightsType;
    private static ConfigEntry<float> JulesLove;
    private static ConfigEntry<float> DerekLove;
    private static ConfigEntry<float> LloydLove;
    private static ConfigEntry<float> GabrielLove;
    private static ConfigEntry<float> SamirLove;
    private static ConfigEntry<float> ArataLove;
    private static ConfigEntry<float> SophieLove;
    private static ConfigEntry<float> JuneLove;
    private static ConfigEntry<float> FreyaLove;
    private static ConfigEntry<float> MapleLove;
    private static ConfigEntry<float> KagetsuLove;
    private static ConfigEntry<float> DianaLove;
    private static ConfigEntry<float> ChildLove;
    private static ConfigEntry<float> FelixLove;
    private static ConfigEntry<float> ErikLove;
    private static ConfigEntry<float> StuartLove;
    private static ConfigEntry<float> SoniaLove;
    private static ConfigEntry<float> MadeleineLove;
    private static ConfigEntry<float> MinaLove;
    private static ConfigEntry<float> WilburLove;
    private static ConfigEntry<float> ClaraLove;
    private static ConfigEntry<float> KevinLove;
    private static ConfigEntry<float> IsaacLove;
    private static ConfigEntry<float> NadineLove;
    private static ConfigEntry<float> SylviaLove;
    private static ConfigEntry<float> LaurieLove;
    private static ConfigEntry<float> MiguelLove;
    private static ConfigEntry<float> HaroldLove;
    private static ConfigEntry<float> ShereneLove;

    public override void Load()
    {
        GrowthType = Config.Bind("--------01 RELATIONSHIP GROWTH--------", "Growth Type", "Default", "Choose the type of growth for your relationships. Growth type should NOT include spaces and is case sensitive.\nDefault: Default growth.\nGentleSlope: Exponential, but less extreme.\nLinear: Mostly balanced, linear growth.\nLinearEasy: Linear growth meant to be easier than default.\nLinearHard: Linear growth meant to be harder than default.\nLinearM: Linear growth meant to give a much larger challenge.\nCustom: Create your own settings.");
        Hearts1 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","One Heart", 1000, "Points required for 1 heart. Only used if Growth Type is set to Custom.");
        Hearts2 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Two Hearts", 3000, "Points required for 2 hearts. Only used if Growth Type is set to Custom.");
        Hearts3 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Three Hearts", 5000, "Points required for 3 hearts. Only used if Growth Type is set to Custom.");
        Hearts4 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Four Hearts", 9000, "Points required for 4 hearts. Only used if Growth Type is set to Custom.");
        Hearts5 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Five Hearts", 13000, "Points required for 5 hearts. Only used if Growth Type is set to Custom.");
        Hearts6 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Six Hearts", 19000, "Points required for 6 hearts. Only used if Growth Type is set to Custom.");
        Hearts7 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Seven Hearts", 25000, "Points required for 7 hearts. Only used if Growth Type is set to Custom.");
        Hearts8 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Eight Hearts", 33000, "Points required for 8 hearts. Only used if Growth Type is set to Custom.");
        Hearts9 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Nine Hearts", 41000, "Points required for 9 hearts. Only used if Growth Type is set to Custom.");
        Hearts10 = Config.Bind("--------01 RELATIONSHIP GROWTH--------","Ten Hearts", 50000, "Points required for 10 hearts. Only used if Growth Type is set to Custom.");
        
        WeightsType = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Weights Type", "Default", "Choose the type of weights for your relationships. Growth type should NOT include spaces and is case sensitive.\nDefault: Everyone gains relationship points equally.\nSmallWeights: Characters are 10% harder/easier to gain relationship with based on personality.\nMediumWeights: Characters are 25% harder/easier to gain relationship with based on personality.\nLargeWeights: Characters are 50% harder/easier to gain relationship with based on personality.\nCustom: Create your own weights.");
        JulesLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Jules Multiplier", 1f, "Multiplier for relationship gain with Jules. Smaller number is harder, larger number is easier.");
        DerekLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Derek Multiplier", 1.1f, "Multiplier for relationship gain with Derek. Smaller number is harder, larger number is easier.");
        LloydLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Lloyd Multiplier", 0.9f, "Multiplier for relationship gain with Lloyd. Smaller number is harder, larger number is easier.");
        GabrielLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Gabriel Multiplier", 1f, "Multiplier for relationship gain with Gabriel. Smaller number is harder, larger number is easier.");
        SamirLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Samir Multiplier", 0.9f, "Multiplier for relationship gain with Samir. Smaller number is harder, larger number is easier.");
        ArataLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Arata Multiplier", 1.1f, "Multiplier for relationship gain with Arata. Smaller number is harder, larger number is easier.");
        SophieLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Sophie Multiplier", 1.1f, "Multiplier for relationship gain with Sophie. Smaller number is harder, larger number is easier.");
        JuneLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "June Multiplier", 0.9f, "Multiplier for relationship gain with June. Smaller number is harder, larger number is easier.");
        FreyaLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Freya Multiplier", 1f, "Multiplier for relationship gain with Freya. Smaller number is harder, larger number is easier.");
        MapleLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Maple Multiplier", 1.1f, "Multiplier for relationship gain with Maple. Smaller number is harder, larger number is easier.");
        KagetsuLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Kagetsu Multiplier", 0.9f, "Multiplier for relationship gain with Kagetsu. Smaller number is harder, larger number is easier.");
        DianaLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Diana Multiplier", 0.9f, "Multiplier for relationship gain with Diana. Smaller number is harder, larger number is easier.");
        ChildLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Child Multiplier", 1.1f, "Multiplier for relationship gain with Child. Smaller number is harder, larger number is easier.");
        FelixLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Felix Multiplier", 1.1f, "Multiplier for relationship gain with Felix. Smaller number is harder, larger number is easier.");
        ErikLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Erik Multiplier", 1f, "Multiplier for relationship gain with Erik. Smaller number is harder, larger number is easier.");
        StuartLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Stuart Multiplier", 1f, "Multiplier for relationship gain with Stuart. Smaller number is harder, larger number is easier.");
        SoniaLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Sonia Multiplier", 1f, "Multiplier for relationship gain with Sonia. Smaller number is harder, larger number is easier.");
        MadeleineLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Madeleine Multiplier", 1f, "Multiplier for relationship gain with Madeleine. Smaller number is harder, larger number is easier.");
        MinaLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Mina Multiplier", 1f, "Multiplier for relationship gain with Mina. Smaller number is harder, larger number is easier.");
        WilburLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Wilbur Multiplier", 1f, "Multiplier for relationship gain with Wilbur. Smaller number is harder, larger number is easier.");
        ClaraLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Clara Multiplier", 1f, "Multiplier for relationship gain with Clara. Smaller number is harder, larger number is easier.");
        KevinLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Kevin Multiplier", 1f, "Multiplier for relationship gain with Kevin. Smaller number is harder, larger number is easier.");
        IsaacLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Isaac Multiplier", 1f, "Multiplier for relationship gain with Isaac. Smaller number is harder, larger number is easier.");
        NadineLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Nadine Multiplier", 1f, "Multiplier for relationship gain with Nadine. Smaller number is harder, larger number is easier.");
        SylviaLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Sylvia Multiplier", 0.9f, "Multiplier for relationship gain with Sylvia. Smaller number is harder, larger number is easier.");
        LaurieLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Laurie Multiplier", 1f, "Multiplier for relationship gain with Laurie. Smaller number is harder, larger number is easier.");
        MiguelLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Miguel Multiplier", 0.9f, "Multiplier for relationship gain with Miguel. Smaller number is harder, larger number is easier.");
        HaroldLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Harold Multiplier", 1f, "Multiplier for relationship gain with Harold. Smaller number is harder, larger number is easier.");
        ShereneLove = Config.Bind("--------02 CHARACTER RELATIONSHIP WEIGHTS--------", "Sherene Multiplier", 1.1f, "Multiplier for relationship gain with Sherene. Smaller number is harder, larger number is easier.");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Premades));
        Harmony.CreateAndPatchAll(typeof(CustomPoints));
        Harmony.CreateAndPatchAll(typeof(WeightPremades));
        Harmony.CreateAndPatchAll(typeof(CustomWeights));
    }

    private static class WeightPremades
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void ApplyWeights()
        {
            var allowedValues = new HashSet<string> { "Default", "SmallWeights", "MediumWeights", "LargeWeights" };
            if (!allowedValues.Contains(WeightsType.Value)) return;
            
            var love = MasterDataManager.Instance.CharacterMaster.list;

            if (WeightsType.Value == "Default")
            {
                foreach (var likelove in love)
                {
                    likelove.LikeAbilityRate = 1f;
                }
            }
            else if (WeightsType.Value == "SmallWeights")
            {
                love[0].LikeAbilityRate = 1f; // Jules
                love[1].LikeAbilityRate = 1.1f; // Derek
                love[2].LikeAbilityRate = 0.9f; // Lloyd
                love[3].LikeAbilityRate = 1f; // Gabriel
                love[4].LikeAbilityRate = 0.9f; // Samir
                love[5].LikeAbilityRate = 1.1f; // Arata
                love[6].LikeAbilityRate = 1.1f; // Sophie
                love[7].LikeAbilityRate = 0.9f; // June
                love[8].LikeAbilityRate = 1f; // Freya
                love[9].LikeAbilityRate = 1.1f; // Maple
                love[10].LikeAbilityRate = 1f; // Kagetsu
                love[11].LikeAbilityRate = 0.9f; // Diana
                love[12].LikeAbilityRate = 1.1f; // Child
                love[13].LikeAbilityRate = 1.1f; // Felix
                love[14].LikeAbilityRate = 1f; // Erik
                love[15].LikeAbilityRate = 1f; // Stuart
                love[16].LikeAbilityRate = 1f; // Sonia
                love[17].LikeAbilityRate = 1f; // Madeleine
                love[18].LikeAbilityRate = 1f; // Mina
                love[19].LikeAbilityRate = 1f; // Wilbur
                love[20].LikeAbilityRate = 1f; // Clara
                love[21].LikeAbilityRate = 1f; // Kevin
                love[22].LikeAbilityRate = 1f; // Isaac
                love[23].LikeAbilityRate = 1f; // Nadine
                love[24].LikeAbilityRate = 0.9f; // Sylvia
                love[25].LikeAbilityRate = 1f; // Laurie
                love[26].LikeAbilityRate = 0.9f; // Miguel
                love[27].LikeAbilityRate = 1f; // Harold
                love[28].LikeAbilityRate = 1.1f; // Sherene
            }
            else if (WeightsType.Value == "MediumWeights")
            {
                love[0].LikeAbilityRate = 1f; // Jules
                love[1].LikeAbilityRate = 1.25f; // Derek
                love[2].LikeAbilityRate = 0.75f; // Lloyd
                love[3].LikeAbilityRate = 1f; // Gabriel
                love[4].LikeAbilityRate = 0.75f; // Samir
                love[5].LikeAbilityRate = 1.25f; // Arata
                love[6].LikeAbilityRate = 1.25f; // Sophie
                love[7].LikeAbilityRate = 0.75f; // June
                love[8].LikeAbilityRate = 1f; // Freya
                love[9].LikeAbilityRate = 1.25f; // Maple
                love[10].LikeAbilityRate = 1f; // Kagetsu
                love[11].LikeAbilityRate = 0.75f; // Diana
                love[12].LikeAbilityRate = 1.25f; // Child
                love[13].LikeAbilityRate = 1.25f; // Felix
                love[14].LikeAbilityRate = 1f; // Erik
                love[15].LikeAbilityRate = 1f; // Stuart
                love[16].LikeAbilityRate = 1f; // Sonia
                love[17].LikeAbilityRate = 1f; // Madeleine
                love[18].LikeAbilityRate = 1f; // Mina
                love[19].LikeAbilityRate = 1f; // Wilbur
                love[20].LikeAbilityRate = 1f; // Clara
                love[21].LikeAbilityRate = 1f; // Kevin
                love[22].LikeAbilityRate = 1f; // Isaac
                love[23].LikeAbilityRate = 1f; // Nadine
                love[24].LikeAbilityRate = 0.75f; // Sylvia
                love[25].LikeAbilityRate = 1f; // Laurie
                love[26].LikeAbilityRate = 0.75f; // Miguel
                love[27].LikeAbilityRate = 1f; // Harold
                love[28].LikeAbilityRate = 1.25f; // Sherene
            }
            else if (WeightsType.Value == "LargeWeights")
            {
                love[0].LikeAbilityRate = 1f; // Jules
                love[1].LikeAbilityRate = 1.5f; // Derek
                love[2].LikeAbilityRate = 0.5f; // Lloyd
                love[3].LikeAbilityRate = 1f; // Gabriel
                love[4].LikeAbilityRate = 0.5f; // Samir
                love[5].LikeAbilityRate = 1.5f; // Arata
                love[6].LikeAbilityRate = 1.5f; // Sophie
                love[7].LikeAbilityRate = 0.5f; // June
                love[8].LikeAbilityRate = 1f; // Freya
                love[9].LikeAbilityRate = 1.5f; // Maple
                love[10].LikeAbilityRate = 1f; // Kagetsu
                love[11].LikeAbilityRate = 0.5f; // Diana
                love[12].LikeAbilityRate = 1.5f; // Child
                love[13].LikeAbilityRate = 1.5f; // Felix
                love[14].LikeAbilityRate = 1f; // Erik
                love[15].LikeAbilityRate = 1f; // Stuart
                love[16].LikeAbilityRate = 1f; // Sonia
                love[17].LikeAbilityRate = 1f; // Madeleine
                love[18].LikeAbilityRate = 1f; // Mina
                love[19].LikeAbilityRate = 1f; // Wilbur
                love[20].LikeAbilityRate = 1f; // Clara
                love[21].LikeAbilityRate = 1f; // Kevin
                love[22].LikeAbilityRate = 1f; // Isaac
                love[23].LikeAbilityRate = 1f; // Nadine
                love[24].LikeAbilityRate = 0.5f; // Sylvia
                love[25].LikeAbilityRate = 1f; // Laurie
                love[26].LikeAbilityRate = 0.5f; // Miguel
                love[27].LikeAbilityRate = 1f; // Harold
                love[28].LikeAbilityRate = 1.5f; // Sherene
            }
        }
    }
    
    private static class CustomWeights
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void ApplyWeights()
        {
            if (WeightsType.Value != "Custom") return;
            
            var love = MasterDataManager.Instance.CharacterMaster.list;
            love[0].LikeAbilityRate = JulesLove.Value;
            love[1].LikeAbilityRate = DerekLove.Value;
            love[2].LikeAbilityRate = LloydLove.Value;
            love[3].LikeAbilityRate = GabrielLove.Value;
            love[4].LikeAbilityRate = SamirLove.Value;
            love[5].LikeAbilityRate = ArataLove.Value;
            love[6].LikeAbilityRate = SophieLove.Value;
            love[7].LikeAbilityRate = JuneLove.Value;
            love[8].LikeAbilityRate = FreyaLove.Value;
            love[9].LikeAbilityRate = MapleLove.Value;
            love[10].LikeAbilityRate = KagetsuLove.Value;
            love[11].LikeAbilityRate = DianaLove.Value;
            love[12].LikeAbilityRate = ChildLove.Value;
            love[13].LikeAbilityRate = FelixLove.Value;
            love[14].LikeAbilityRate = ErikLove.Value;
            love[15].LikeAbilityRate = StuartLove.Value;
            love[16].LikeAbilityRate = SoniaLove.Value;
            love[17].LikeAbilityRate = MadeleineLove.Value;
            love[18].LikeAbilityRate = MinaLove.Value;
            love[19].LikeAbilityRate = WilburLove.Value;
            love[20].LikeAbilityRate = ClaraLove.Value;
            love[21].LikeAbilityRate = KevinLove.Value;
            love[22].LikeAbilityRate = IsaacLove.Value;
            love[23].LikeAbilityRate = NadineLove.Value;
            love[24].LikeAbilityRate = SylviaLove.Value;
            love[25].LikeAbilityRate = LaurieLove.Value;
            love[26].LikeAbilityRate = MiguelLove.Value;
            love[27].LikeAbilityRate = HaroldLove.Value;
            love[28].LikeAbilityRate = ShereneLove.Value;
        }
    }
    
    private class Premades
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        public static void PremadeGrowth()
        {
            var allowedValues = new HashSet<string> { "Default", "GentleSlope", "Linear", "LinearEasy", "LinearHard", "LinearM" };
            if (!allowedValues.Contains(GrowthType.Value)) return;

            var like = SettingAssetManager.Instance.LikeabilitySetting;
            
            if (GrowthType.Value == "Default")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 1000;
                like.LevelList[2] = 3000;
                like.LevelList[3] = 5000;
                like.LevelList[4] = 9000;
                like.LevelList[5] = 13000;
                like.LevelList[6] = 19000;
                like.LevelList[7] = 25000;
                like.LevelList[8] = 33000;
                like.LevelList[9] = 41000;
                like.LevelList[10] = 50000;
            }
            else if (GrowthType.Value == "GentleSlope")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 1750;
                like.LevelList[2] = 4000;
                like.LevelList[3] = 6750;
                like.LevelList[4] = 10000;
                like.LevelList[5] = 13750;
                like.LevelList[6] = 18000;
                like.LevelList[7] = 22750;
                like.LevelList[8] = 28000;
                like.LevelList[9] = 33750;
                like.LevelList[10] = 40000;
            }
            else if (GrowthType.Value == "Linear")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 4000;
                like.LevelList[2] = 8000;
                like.LevelList[3] = 12000;
                like.LevelList[4] = 16000;
                like.LevelList[5] = 20000;
                like.LevelList[6] = 24000;
                like.LevelList[7] = 28000;
                like.LevelList[8] = 32000;
                like.LevelList[9] = 36000;
                like.LevelList[10] = 40000;
            }
            else if (GrowthType.Value == "LinearEasy")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 2000;
                like.LevelList[2] = 4000;
                like.LevelList[3] = 6000;
                like.LevelList[4] = 8000;
                like.LevelList[5] = 10000;
                like.LevelList[6] = 12000;
                like.LevelList[7] = 14000;
                like.LevelList[8] = 16000;
                like.LevelList[9] = 18000;
                like.LevelList[10] = 20000;
            }
            else if (GrowthType.Value == "LinearHard")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 6000;
                like.LevelList[2] = 12000;
                like.LevelList[3] = 18000;
                like.LevelList[4] = 24000;
                like.LevelList[5] = 30000;
                like.LevelList[6] = 36000;
                like.LevelList[7] = 42000;
                like.LevelList[8] = 48000;
                like.LevelList[9] = 54000;
                like.LevelList[10] = 60000;
            }
            else if (GrowthType.Value == "LinearM")
            {
                like.LevelList[0] = 0;
                like.LevelList[1] = 10000;
                like.LevelList[2] = 20000;
                like.LevelList[3] = 30000;
                like.LevelList[4] = 40000;
                like.LevelList[5] = 50000;
                like.LevelList[6] = 60000;
                like.LevelList[7] = 70000;
                like.LevelList[8] = 80000;
                like.LevelList[9] = 90000;
                like.LevelList[10] = 100000;
            }
        }
    }

    private class CustomPoints
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        public static void CustomGrowth()
        {
            if (GrowthType.Value != "Custom") return;
            
            var like = SettingAssetManager.Instance.LikeabilitySetting;

            like.LevelList[0] = Hearts1.Value;
            like.LevelList[1] = Hearts2.Value;
            like.LevelList[2] = Hearts3.Value;
            like.LevelList[3] = Hearts4.Value;
            like.LevelList[4] = Hearts5.Value;
            like.LevelList[5] = Hearts6.Value;
            like.LevelList[6] = Hearts7.Value;
            like.LevelList[7] = Hearts8.Value;
            like.LevelList[8] = Hearts9.Value;
            like.LevelList[9] = Hearts10.Value;
        }
    }
}