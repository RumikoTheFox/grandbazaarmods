using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BokuMono;
using HarmonyLib;

namespace Cookbook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    
    private static ConfigEntry<bool> AllRecipes;
    private static ConfigEntry<bool> Salads;
    private static ConfigEntry<bool> Soups;
    private static ConfigEntry<bool> Sides;
    private static ConfigEntry<bool> Entrees;
    private static ConfigEntry<bool> Desserts;
    private static ConfigEntry<bool> Others;
    private static ConfigEntry<bool> CafeMad;
    private static ConfigEntry<bool> MiniMad;
    private static ConfigEntry<bool> Clara;
    private static ConfigEntry<bool> Nadine;
    private static ConfigEntry<bool> Felix;
    private static ConfigEntry<bool> Sprites;
    private static ConfigEntry<bool> Requests;
    private static ConfigEntry<bool> Holidays;
    private static ConfigEntry<bool> Search;
    private static ConfigEntry<bool> Achievements;
    private static ConfigEntry<bool> Unknown;
    private static ConfigEntry<bool> Missions;
    private static ConfigEntry<bool> FavGifts;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(CookbookRecipes));
        
        AllRecipes = Config.Bind("All", "All Recipes", false, "Set true to learn all recipes.");
        Salads = Config.Bind("Categories", "All Salads", false, "Set true to learn all \"Salads\" recipes.");
        Soups = Config.Bind("Categories", "All Soups", false, "Set true to learn all \"Soups\" recipes.");
        Sides = Config.Bind("Categories", "All Sides", false, "Set true to learn all \"Sides\" recipes.");
        Entrees = Config.Bind("Categories", "All Main Dishes", false, "Set true to learn all \"Main Dishes\" recipes.");
        Desserts = Config.Bind("Categories", "All Desserts", false, "Set true to learn all \"Desserts\" recipes.");
        Others = Config.Bind("Categories", "All Other Recipes", false, "Set true to learn all \"Other Recipes\" recipes.");
        CafeMad = Config.Bind("Source", "All Cafe Madeleine Recipes", false, "Set true to learn all Cafe Madeleine recipes.");
        MiniMad = Config.Bind("Source", "All Mini Madeleine Recipes", false, "Set true to learn all Mini Madeleine recipes.");
        Clara = Config.Bind("Source", "All Clara’s Diner Recipes", false, "Set true to learn all Clara's Diner recipes.");
        Nadine = Config.Bind("Source", "All Nadine’s Bistro Recipes", false, "Set true to learn all Nadine's Bistro recipes.");
        Felix = Config.Bind("Source", "All Felix’s Fixings Recipes", false, "Set true to learn all Felix's Fixings recipes.");
        Sprites = Config.Bind("Source", "All Happy Sprites Store Recipes", false, "Set true to learn all Happy Sprites Store recipes.");
        Requests = Config.Bind("Source", "All Request Reward Recipes", false, "Set true to learn all request reward recipes.");
        Holidays = Config.Bind("Source", "All Holiday Recipes", false, "Set true to learn all recipes earned from holidays.");
        Search = Config.Bind("Source", "All Bookshelf Recipes", false, "Set true to learn all recipes found by snooping in your neighbors' bookshelves.");
        Achievements = Config.Bind("Source", "All Achievement Reward Recipes", false, "Set true to learn all recipes earned via achievements.");
        Unknown = Config.Bind("Source", "All Unknown Recipes", false, "Set true to learn all recipes I couldn't find the source for: Dry Curry, Fried Rice Noodles, Golden Blend Tea, Rüeblitorte.");
        Missions = Config.Bind("Use", "All Mission Objective Recipes", false, "Set true to learn all recipes needed for certain requests.");
        FavGifts = Config.Bind("Use", "All Favorite Gifts Recipes", false, "Set true to learn all recipes that are someone's favorite gift.");
    }
    
    public static class RecipeGroups {
        public static readonly List<uint> SaladsList = new() { 118001, 118002, 118003, 118004, 118005, 118006, 118009, 118566, 118567, 118568, 118568 };
        public static readonly List<uint> SoupsList = new() { 118100, 118101, 118103, 118104, 118105, 118107, 118108, 118111 };
        public static readonly List<uint> SidesList = new() { 118200, 118201, 118202, 118203, 118204, 118205, 118206, 118207, 118208, 118209, 118211, 118213, 118214, 118215, 118217, 118218, 118219, 118222, 118223, 118231, 118232, 118233, 118234, 118235, 118236, 118237, 118238, 118239, 118240, 118241, 118242, 118243, 118244, 118246, 118247, 118249, 118250, 118251, 118252, 118253, 118254, 118256, 118257, 118258, 118259, 118260, 118261, 118502, 118563, 118564, 118565, 118579 };
        public static readonly List<uint> EntreesList = new() { 118007, 118102, 118112, 118210, 118220, 118221, 118224, 118225, 118226, 118227, 118228, 118229, 118230, 118245, 118255, 118300, 118301, 118303, 118304, 118305, 118306, 118307, 118308, 118309, 118310, 118311, 118312, 118314, 118315, 118316, 118317, 118319, 118320, 118321, 118322, 118323, 118324, 118325, 118326, 118327, 118328, 118329, 118330, 118331, 118332, 118333, 118334, 118335, 118336, 118337, 118338, 118339, 118340, 118341, 118342, 118343, 118344, 118345, 118346, 118347, 118348, 118349, 118350, 118351, 118352, 118353, 118354, 118355, 118356, 118569, 118570, 118573, 118574, 118575, 118576, 118577, 118578 };
        public static readonly List<uint> DessertsList = new() { 118400, 118401, 118402, 118403, 118404, 118405, 118406, 118407, 118408, 118409, 118409, 118410, 118411, 118411, 118412, 118413, 118414, 118415, 118416, 118417, 118417, 118418, 118419, 118420, 118421, 118421, 118422, 118423, 118424, 118425, 118426, 118427, 118428, 118429, 118430, 118431, 118432, 118433, 118434, 118435, 118435, 118436, 118437, 118438, 118571, 118572 };
        public static readonly List<uint> OthersList = new() { 118110, 118248, 118504, 118505, 118506, 118507, 118508, 118509, 118510, 118510, 118511, 118512, 118513, 118514, 118515, 118516, 118517, 118519, 118520, 118521, 118522, 118523, 118524, 118525, 118526, 118527, 118528, 118529, 118530, 118531, 118532, 118533, 118534, 118535, 118536, 118537, 118538, 118539, 118540, 118541, 118542, 118543, 118544, 118545, 118546, 118547, 118548, 118549, 118550, 118551, 118552, 118553, 118554, 118555, 118556, 118557, 118558, 118559, 118560, 118561, 118562, 118580, 118581, 118582, 118583, 118584, 118585 };
        public static readonly List<uint> CafeMadList = new() { 118001, 118003, 118101, 118110, 118112, 118200, 118202, 118213, 118214, 118224, 118228, 118234, 118260, 118300, 118320, 118324, 118350, 118402, 118407, 118414, 118416, 118417, 118425, 118508, 118509, 118519, 118530, 118531, 118541, 118543, 118544, 118545, 118554, 118555, 118573 };
        public static readonly List<uint> MiniMadList = new() { 118002, 118005, 118102, 118103, 118104, 118201, 118203, 118207, 118218, 118225, 118229, 118240, 118246, 118247, 118250, 118259, 118315, 118319, 118322, 118325, 118340, 118352, 118354, 118355, 118418, 118423, 118429, 118431, 118435, 118438, 118511, 118514, 118515, 118516, 118517, 118521, 118522, 118523, 118532, 118533, 118534, 118535, 118539, 118549, 118550, 118551, 118552, 118553, 118559, 118560, 118562, 118567, 118579, 118581 };
        public static readonly List<uint> ClaraList = new() { 118009, 118107, 118111, 118208, 118215, 118222, 118227, 118233, 118235, 118239, 118241, 118245, 118248, 118249, 118251, 118253, 118254, 118256, 118257, 118258, 118303, 118311, 118312, 118314, 118323, 118329, 118331, 118332, 118333, 118335, 118338, 118343, 118344, 118345, 118346, 118347, 118348, 118349, 118356, 118409, 118410, 118419, 118424, 118430, 118526, 118527, 118528, 118557, 118566, 118574, 118576 };
        public static readonly List<uint> NadineList = new() { 118004, 118100, 118209, 118223, 118230, 118232, 118236, 118237, 118244, 118252, 118261, 118304, 118305, 118306, 118307, 118308, 118310, 118316, 118327, 118328, 118339, 118400, 118401, 118408, 118411, 118413, 118421, 118426, 118428, 118432, 118436, 118502, 118504, 118505, 118506, 118507, 118510, 118512, 118513, 118520, 118525, 118536, 118537, 118538, 118542, 118547, 118548, 118558, 118561, 118564, 118565, 118568, 118569, 118570, 118571, 118575, 118577, 118578, 118580 };
        public static readonly List<uint> FelixList = new() { 118206, 118210, 118211, 118219, 118220, 118403 };
        public static readonly List<uint> SpritesList = new() { 118582, 118583, 118584, 118585 };
        public static readonly List<uint> RequestsList = new() { 118006, 118007, 118105, 118108, 118204, 118205, 118217, 118226, 118238, 118243, 118255, 118317, 118321, 118326, 118330, 118336, 118337, 118341, 118404, 118406, 118433, 118434, 118437, 118529, 118540, 118563 };
        public static readonly List<uint> HolidaysList = new() { 118200, 118201, 118202, 118203, 118204, 118205, 118206, 118211, 118215, 118220, 118221, 118235, 118236, 118238, 118240, 118247, 118304, 118305, 118307, 118308, 118309, 118321, 118322, 118323, 118324, 118325, 118329, 118330, 118417, 118422, 118510 };
        public static readonly List<uint> SearchList = new() { 118231, 118242, 118301, 118351, 118420, 118556 };
        public static readonly List<uint> AchievementsList = new() { 118207, 118223, 118310, 118311, 118342, 118405, 118409, 118411, 118412, 118415, 118421, 118427, 118435, 118524, 118568 };
        public static readonly List<uint> UnknownList = new() { 118334, 118353, 118546, 118572 };
        public static readonly List<uint> MissionsList = new() { 118213, 118225, 118230, 118301, 118314, 118335, 118411, 118411, 118416, 118509, 118530, 118554 };
        public static readonly List<uint> FavGiftsList = new() { 118221, 118225, 118228, 118235, 118235, 118236, 118236, 118244, 118301, 118337, 118338, 118341, 118351, 118355, 118400, 118404, 118411, 118411, 118415, 118420, 118431, 118436, 118438, 118535, 118543, 118571, 118576 };
    }

    private class CookbookRecipes
    {
        [HarmonyPatch(typeof(CookingManager), "FromSaveData")]
        [HarmonyPrefix]
        private static void Prefix(CookingManager __instance, CookingManager.SaveData saveData)
        {
            HashSet<uint> masterSet = new();
            if (Salads.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.SaladsList);
            if (Soups.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.SoupsList);
            if (Sides.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.SidesList);
            if (Entrees.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.EntreesList);
            if (Desserts.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.DessertsList);
            if (Others.Value || AllRecipes.Value) masterSet.UnionWith(RecipeGroups.OthersList);
            if (CafeMad.Value) masterSet.UnionWith(RecipeGroups.CafeMadList);
            if (MiniMad.Value) masterSet.UnionWith(RecipeGroups.MiniMadList);
            if (Clara.Value) masterSet.UnionWith(RecipeGroups.ClaraList);
            if (Nadine.Value) masterSet.UnionWith(RecipeGroups.NadineList);
            if (Felix.Value) masterSet.UnionWith(RecipeGroups.FelixList);
            if (Sprites.Value) masterSet.UnionWith(RecipeGroups.SpritesList);
            if (Requests.Value) masterSet.UnionWith(RecipeGroups.RequestsList);
            if (Holidays.Value) masterSet.UnionWith(RecipeGroups.HolidaysList);
            if (Search.Value) masterSet.UnionWith(RecipeGroups.SearchList);
            if (Achievements.Value) masterSet.UnionWith(RecipeGroups.AchievementsList);
            if (Unknown.Value) masterSet.UnionWith(RecipeGroups.UnknownList);
            if (Missions.Value) masterSet.UnionWith(RecipeGroups.MissionsList);
            if (FavGifts.Value) masterSet.UnionWith(RecipeGroups.FavGiftsList);

            foreach (uint recipeId in masterSet)
            {
                if (!saveData.LearnedRecipeId.Contains(recipeId))
                {
                    saveData.LearnedRecipeId.Add(recipeId);
                }
            }
        }
    }
}