using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono;
using BokuMono.Data;
using HarmonyLib;
using BepInEx.Configuration;
using Il2CppSystem;

namespace WindmillOverhaul;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    private static ConfigEntry<int> CraftSlots1;
    private static ConfigEntry<int> CraftSlots2;
    private static ConfigEntry<int> CraftSize1;
    private static ConfigEntry<int> CraftSize2;
    private static ConfigEntry<float> WindStrength0;
    private static ConfigEntry<float> WindStrength1;
    private static ConfigEntry<float> WindStrength2;
    private static ConfigEntry<float> WindStrength3;
    private static ConfigEntry<int> YellowCount;
    private static ConfigEntry<int> YellowInterval;
    private static ConfigEntry<float> RedQuality;
    private static ConfigEntry<bool> AllRecipes;
    private static ConfigEntry<bool> ModifiedWindscreen;
    private static ConfigEntry<int> WindscreenCost;
    private static ConfigEntry<bool> ModifiedMaterials;
    private static ConfigEntry<int> ForStone;
    private static ConfigEntry<int> ForSturdyStone;
    private static ConfigEntry<int> ForUltimateStone;
    private static ConfigEntry<int> ForLumber;
    private static ConfigEntry<int> ForSturdyLumber;
    private static ConfigEntry<int> ForUltimateLumber;
    private static ConfigEntry<bool> CheaperRecipesToggle;
    private static ConfigEntry<int> CheaperRecipeItem;
    private static ConfigEntry<bool> CraftTimeToggle;
    private static ConfigEntry<int> CraftTime;
    private static ConfigEntry<bool> HiddenItems;
    private static ConfigEntry<float> BlueTime;
    private static ConfigEntry<bool> AllowSkyJewel;
    private static ConfigEntry<bool> AllowMatsutake;
    private static ConfigEntry<bool> UpgradeCrops;
    private static ConfigEntry<bool> CheaperSprinker;
    internal static new ManualLogSource Log;

    public override void Load()
    {
        CraftSlots1 = Config.Bind("--------06 PRODUCTION SLOTS--------", "Production Slots",72, 
            "Set your preferred number of production slots. Game Default: 6");
        CraftSlots2 = Config.Bind("--------06 PRODUCTION SLOTS--------", "Production Slots Upgrade",72, 
            "Set your preferred number of production slots with a Green Wonderstone. Game Default: 12");
        CraftSize1 = Config.Bind("--------07 CRAFTING STACKS--------", "Craft Stack",10, 
            "Set the max amount you can craft of an object per production slot. Game Default: 10");
        CraftSize2 = Config.Bind("--------07 CRAFTING STACKS--------", "Craft Stack Upgrade",30, 
            "Set the max amount you can craft of an object per production slot with an Orange Wonderstone. Game Default: 30");
        WindStrength0 = Config.Bind("--------13 WIND SPEED--------", "Wind Speed: Normal",1.0f, 
            "Set the multiplier for crafting time when no wind is blowing. Game Default: 1.0");
        WindStrength1 = Config.Bind("--------13 WIND SPEED--------", "Wind Speed: Breeze",0.75f, 
            "Set the multiplier for crafting time when a light wind is blowing. Game Default: 1.0");
        WindStrength2 = Config.Bind("--------13 WIND SPEED--------", "Wind Speed: Gust",0.5f, 
            "Set the multiplier for crafting time when a heavy wind is blowing. Game Default: 0.75");
        WindStrength3 = Config.Bind("--------13 WIND SPEED--------", "Wind Speed: Typhoon",0.25f, 
            "Set the multiplier for crafting time when there is a typhoon. Game Default: 0.5");
        YellowCount = Config.Bind("--------08 YELLOW WONDERSTONE BUFF--------", "Freebie Amount", 1, 
            "Set to the number of items you'd like the Yellow Wonderstone to give you when it gives an extra item. Game Default: 1");
        YellowInterval = Config.Bind("--------08 YELLOW WONDERSTONE BUFF--------", "Items Per Freebie", 5, 
            "Set to the number of items that must be crafted at once for a freebie. Keep in mind that depending on the number versus your max crafting\n" +
            "stack, it can fire off more than once. So if you set this to 5 and you can craft 30 items at a time, that stack will give you 6 freebie items. Game Default: 5");
        RedQuality = Config.Bind("--------09 RED WONDERSTONE BUFF--------", "Red Stone Quality Increase",1.0f, 
            "Set the quality increase for making crops into seeds when done with a Red Wonderstone. Game Default: 1.0");
        BlueTime = Config.Bind("--------10 BLUE WONDERSTONE BUFF--------", "Blue Stone Craft Multiplier",0.5f, 
            "Set the Blue Wonderstone's crafting time multiplier. Lower number means faster. Game Default: 0.5");
        AllRecipes = Config.Bind("--------02 ALL RECIPES--------", "All Recipes Enabled", false,
            "Set to true to unlock all recipes in all windmills.");
        ModifiedWindscreen = Config.Bind("--------03 MODIFIED RECIPE COSTS--------", "Cheaper Windscreen Recipe", false,
            "Set to true to enable cheaper Windscreen Kit crafting.");
        WindscreenCost = Config.Bind("--------03 MODIFIED RECIPE COSTS--------", "Windscreen Material Cost", 5,
            "Set to the amount each of wood and rocks you want the Windscreen Kit to cost.");
        ModifiedMaterials = Config.Bind("--------04 LUMBER/STONE COST--------", "Modified Material Recipes", false,
            "Set to true to enable modified Lumber/Stone crafting.");
        ForStone = Config.Bind("--------04 LUMBER/STONE COST--------", "Stone Cost", 5,
            "Set the amount of rocks required for this recipe. Balanced: 5. Half Cost: 3.");
        ForSturdyStone = Config.Bind("--------04 LUMBER/STONE COST--------", "Sturdy Stone Cost", 25,
            "Set the amount of rocks required for this recipe. Balanced: 25. Half Cost: 12.");
        ForUltimateStone = Config.Bind("--------04 LUMBER/STONE COST--------", "Ultimate Stone Cost", 125,
            "Set the amount of rocks required for this recipe. Balanced: 125. Half Cost: 60.");
        ForLumber = Config.Bind("--------04 LUMBER/STONE COST--------", "Lumber Cost", 5,
            "Set the amount of wood required for this recipe. Balanced: 5. Half Cost: 3.");
        ForSturdyLumber = Config.Bind("--------04 LUMBER/STONE COST--------", "Sturdy Lumber Cost", 25,
            "Set the amount of wood required for this recipe. Balanced: 25. Half Cost: 12.");
        ForUltimateLumber = Config.Bind("--------04 LUMBER/STONE COST--------", "Ultimate Lumber Cost", 125,
            "Set the amount of wood required for this recipe. Balanced: 125. Half Cost: 60.");
        CheaperRecipesToggle = Config.Bind("--------05 ALL RECIPES CHEAP--------", "All Crafting Cheap", false,
            "Set to true to make all windmill crafting cost a single item. CAUTION: THIS WILL OVERWRITE\n" +
            "ANY RECIPE CHANGES FROM THE MODIFIED RECIPE COSTS SECTION!!!!");
        UpgradeCrops = Config.Bind("--------05 ALL RECIPES CHEAP--------", "Exclude Crops", true,
            "Set to false if you want crops to also be made cheap. Set to true if you want to still be able to use seed bags to increase crop quality.");
        CheaperRecipeItem = Config.Bind("--------05 ALL RECIPES CHEAP--------", "Material Item", 110500,
            "Set this to the ID for the item you want all windmill recipes to cost. Set to 110500, Weed, by default.\n" +
            "Check the IDs at https://docs.google.com/spreadsheets/d/1nvusDeTcFZhyxqV6h8OEBClQtV6VNtMd2WrpcxHMbaE/edit?usp=sharing for\n" +
            "Item IDs if you want to use a different item. Another good candidate is rocks (id 110000).");
        CraftTimeToggle = Config.Bind("--------12 BASE CRAFTING TIME--------", "Change Base Craft Time", false,
            "Set to true to set the base craft time for all recipes at once.");
        CraftTime = Config.Bind("--------12 BASE CRAFTING TIME--------", "Base Crafting Time", 0,
            "Sets the base crafting time of all recipes to this number. Note that this is base time, before\n" +
            "things like wind speed or the Blue Wonderstone affect it.");
        HiddenItems = Config.Bind("--------11 SHOW HIDDEN RECIPES--------", "Show Hidden Recipes", false,
            "Set to true to show all hidden recipes without a Purple Wonderstone.");
        AllowSkyJewel = Config.Bind("--------01 NEW RECIPES--------", "Add Sky Jewel Recipe", true, "Adds a recipe to the third Windmill for the Sky Jewel. Requires a purple wonderstone by default. Various settings from this mod, such as instant craft, easy materials, and adding all recipes to all windmills still work with this recipe.");
        AllowMatsutake = Config.Bind("--------01 NEW RECIPES--------", "Add Second Matsutake Mushroom Spores Recipe", true, "Adds an extra recipe to the second Windmill for Matsutake Mushroom Spores, using alternate materials. Various settings from this mod, such as instant craft, easy materials, and adding all recipes to all windmills still work with this recipe.");
        CheaperSprinker = Config.Bind("--------03 MODIFIED RECIPE COSTS--------", "Cheaper Sprinkler Recipe", false,
            "Removes the gold and silver costs from the Sprinkler.");
        
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(AddSkyJewel));
        Harmony.CreateAndPatchAll(typeof(AddMatsutake));
        Harmony.CreateAndPatchAll(typeof(GameSettingStuff));
        Harmony.CreateAndPatchAll(typeof(WindmillPatch));
        Harmony.CreateAndPatchAll(typeof(WindscreenRecipe));
        Harmony.CreateAndPatchAll(typeof(CheaperSprinkler));
        Harmony.CreateAndPatchAll(typeof(MaterialRecipes));
        Harmony.CreateAndPatchAll(typeof(AllRecipesCheaper));
        Harmony.CreateAndPatchAll(typeof(CraftTimeAndHidden));
    }

    private static class CheaperSprinkler
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void MakeCheaperSprinkler()
        {
            if (CheaperRecipesToggle.Value || !CheaperSprinker.Value) return;
            var windCraftData = MasterDataManager.Instance.WindmillCraftingMasterData;

            var sprinklerRecipe = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                (x => x.CraftingItemId == 121700));

            sprinklerRecipe.RequiredItemId[0] = 112101;
            sprinklerRecipe.RequiredItemStack[0] = 20;
            
            sprinklerRecipe.RequiredItemId[1] = 112100;
            sprinklerRecipe.RequiredItemStack[1] = 30;
                
            sprinklerRecipe.RequiredItemId[2] = 0;
            sprinklerRecipe.RequiredItemStack[2] = 0;
            
            sprinklerRecipe.RequiredItemId[3] = 0;
            sprinklerRecipe.RequiredItemStack[3] = 0;
        }
    }

    private static class AddSkyJewel
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void AddSkyJewelToWindmill()
        {
            if (!AllowSkyJewel.Value) return;
            var addNewRecipes = MasterDataManager.Instance.WindmillCraftingMasterData;
            var newWindRecipes = new Il2CppSystem.Collections.Generic.List<WindmillCraftingMasterData>();
            
            var requiredItemType = new Il2CppSystem.Collections.Generic.List<RequiredItemType>();
            for (int i = 0; i < 5; i++) requiredItemType.Add(RequiredItemType.Item);
            
            var requiredItemId = new Il2CppSystem.Collections.Generic.List<uint>();
            requiredItemId.Add(112106);
            for (int i = 1; i < 5; i++) requiredItemId.Add(0);
            
            var requiredItemStack = new Il2CppSystem.Collections.Generic.List<int>();
            requiredItemStack.Add(1);
            for (int i = 1; i < 5; i++) requiredItemStack.Add(0);

            var recipe = new WindmillCraftingMasterData
            {
                Id = 30780,
                RequiredItemId = requiredItemId,
                RequiredItemStack = requiredItemStack,
                RequiredItemType = requiredItemType,
                RequiredItemQuality = new Il2CppSystem.Collections.Generic.List<int>(),
                RequiredItemFreshness = new Il2CppSystem.Collections.Generic.List<int>(),
                CraftingItemId = 121503,
                CraftCategory = WindmillCraftCategory.Category09,
                Time = 300,
                WindmillType = 3,
                ToolType = ToolType.None,
                ToolLv = 0,
                IsHidden = true
            };

            var prop = typeof(WindmillCraftingMasterData).GetProperty("IsOnce");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(recipe, true);
            }

            newWindRecipes.Add(recipe);
            
            foreach (var r in newWindRecipes)
            {
                addNewRecipes.Add(r);
            }
        }
    }
    
    private static class AddMatsutake
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void AddMatsutakeToWindmill()
        {
            if (!AllowMatsutake.Value) return;
            var addNewRecipes = MasterDataManager.Instance.WindmillCraftingMasterData;
            var newWindRecipes = new Il2CppSystem.Collections.Generic.List<WindmillCraftingMasterData>();
            
            var requiredItemType = new Il2CppSystem.Collections.Generic.List<RequiredItemType>();
            for (int i = 0; i < 5; i++) requiredItemType.Add(RequiredItemType.Item);
            
            var requiredItemId = new Il2CppSystem.Collections.Generic.List<uint>();
            requiredItemId.Add(103000);
            requiredItemId.Add(105305);
            for (int i = 2; i < 5; i++) requiredItemId.Add(0);
            
            var requiredItemStack = new Il2CppSystem.Collections.Generic.List<int>();
            requiredItemStack.Add(1);
            requiredItemStack.Add(1);
            for (int i = 2; i < 5; i++) requiredItemStack.Add(0);

            var recipe = new WindmillCraftingMasterData
            {
                Id = 30790,
                RequiredItemId = requiredItemId,
                RequiredItemStack = requiredItemStack,
                RequiredItemType = requiredItemType,
                RequiredItemQuality = new Il2CppSystem.Collections.Generic.List<int>(),
                RequiredItemFreshness = new Il2CppSystem.Collections.Generic.List<int>(),
                CraftingItemId = 111105,
                CraftCategory = WindmillCraftCategory.Category08,
                Time = 360,
                WindmillType = 2,
                ToolType = ToolType.None,
                ToolLv = 0,
                IsHidden = true
            };

            var prop = typeof(WindmillCraftingMasterData).GetProperty("IsOnce");
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(recipe, true);
            }

            newWindRecipes.Add(recipe);
            
            foreach (var r in newWindRecipes)
            {
                addNewRecipes.Add(r);
            }
        }
    }

    private static class WindmillPatch
    {
        private static Dictionary<uint, int> windmillIds = new() { {120000, 1}, {120010, 3}, {120020, 2} };

        [HarmonyPatch(typeof(FieldManager), "ChangeField", 
            typeof(FieldMasterId), typeof(string), typeof(bool), typeof(Il2CppSystem.Action), typeof(Il2CppSystem.Action<uint>), typeof(bool))]
        [HarmonyPostfix]
        
        private static void Postfix(FieldManager __instance, FieldMasterId fieldId)
        {
            if (!AllRecipes.Value) return;
            
            if (!windmillIds.ContainsKey((uint)fieldId)) return;

            var windData = MasterDataManager.Instance.WindmillCraftingMasterData;

            foreach (var item in windData)
            {
                if(item.CraftingItemId != 103000) item.WindmillType = windmillIds.GetValueSafe((uint)fieldId); // No fertilizers
            }

            // Sort List
            var sortMe = new List<WindmillCraftingMasterData>(windData.ToArray())
                .OrderBy(x => x.CraftCategory).ThenBy(x => x.CraftingItemId);
            windData.Clear();

            foreach (var item in sortMe)
            {
                windData.Add(item);
            }
        }
    }

    private static class WindscreenRecipe
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void WindscreenEdit()
        {
            if (CheaperRecipesToggle.Value) return;
            
            if (ModifiedWindscreen.Value)
            {
                var windCraftData = MasterDataManager.Instance.WindmillCraftingMasterData;

                var result = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 121900));
            
                result.RequiredItemId[0] = 110100;
                result.RequiredItemStack[0] = WindscreenCost.Value;
            
                result.RequiredItemId[1] = 110000;
                result.RequiredItemStack[1] = WindscreenCost.Value;
                
                result.RequiredItemId[2] = 0;
                result.RequiredItemStack[2] = 0;
            }
        }
    }

    private static class MaterialRecipes
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void MaterialCostEdit()
        {
            if (CheaperRecipesToggle.Value) return;
            
            if (ModifiedMaterials.Value)
            {
                var windCraftData = MasterDataManager.Instance.WindmillCraftingMasterData;

                var stonecost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112000));
                var sstonecost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112002));
                var ustonecost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112004));
                var lumbercost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112001));
                var slumbercost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112003));
                var ulumbercost = windCraftData.Find((Predicate<WindmillCraftingMasterData>)
                    (x => x.CraftingItemId == 112005));
            
                stonecost.RequiredItemId[0] = 110000;
                stonecost.RequiredItemStack[0] = ForStone.Value;
                sstonecost.RequiredItemId[0] = 110000;
                sstonecost.RequiredItemStack[0] = ForSturdyStone.Value;
                ustonecost.RequiredItemId[0] = 110000;
                ustonecost.RequiredItemStack[0] = ForUltimateStone.Value;
                lumbercost.RequiredItemId[0] = 110100;
                lumbercost.RequiredItemStack[0] = ForLumber.Value;
                slumbercost.RequiredItemId[0] = 110100;
                slumbercost.RequiredItemStack[0] = ForSturdyLumber.Value;
                ulumbercost.RequiredItemId[0] = 110100;
                ulumbercost.RequiredItemStack[0] = ForUltimateLumber.Value;
            }
        }
    }

    private static class AllRecipesCheaper
    {
        // Map crafted item → custom first ingredient
        private static readonly Dictionary<uint, uint> CustomFirstItemByCraftedItem = new Dictionary<uint, uint>
        {
            { 101, 100 }, // Copper Hatchet uses Hatchet
            { 102, 101 }, // Silver Hatchet uses Copper Hatchet
            { 103, 102 }, // Golden Hatchet uses Silver Hatchet
            { 104, 103 }, // Orichalcum Hatchet uses Golden Hatchet
            { 105, 104 }, // Ultimate Hatchet uses Orichalcum Hatchet
            { 202, 201 }, // Copper Sickle uses Sickle
            { 203, 202 }, // Copper Sickle+ uses Copper Sickle
            { 204, 203 }, // Silver Sickle uses Copper Sickle+
            { 205, 204 }, // Silver Sickle+ uses Silver Sickle
            { 206, 205 }, // Golden Sickle uses Silver Sickle+
            { 207, 206 }, // Golden Sickle+ uses Golden Sickle
            { 208, 207 }, // Orichalcum Sickle uses Golden Sickle+
            { 210, 208 }, // Ultimate Sickle uses Orichalcum Sickle
            { 502, 501 }, // Copper Hoe uses Hoe
            { 503, 502 }, // Copper Hoe+ uses Copper Hoe
            { 504, 503 }, // Silver Hoe uses Copper Hoe+
            { 505, 504 }, // Silver Hoe+ uses Silver Hoe
            { 506, 505 }, // Golden Hoe uses Silver Hoe+
            { 507, 506 }, // Golden Hoe+ uses Golden Hoe
            { 508, 507 }, // Orichalcum Hoe uses Golden Hoe+
            { 510, 508 }, // Ultimate Hoe uses Orichalcum Hoe
            { 602, 601 }, // Copper Watering Can uses Watering Can
            { 603, 602 }, // Copper Watering Can+ uses Copper Watering Can
            { 604, 603 }, // Silver Watering Can uses Copper Watering Can+
            { 605, 604 }, // Silver Watering Can+ uses Silver Watering Can
            { 606, 605 }, // Golden Watering Can uses Silver Watering Can+
            { 607, 606 }, // Golden Watering Can+ uses Golden Watering Can
            { 608, 607 }, // Orichalcum Watering Can uses Golden Watering Can+
            { 610, 608 }, // Ultimate Watering Can uses Orichalcum Watering Can
            { 702, 701 }, // Copper Fishing Rod uses Fishing Rod
            { 703, 702 }, // Copper Fishing Rod+ uses Copper Fishing Rod
            { 704, 703 }, // Silver Fishing Rod uses Copper Fishing Rod+
            { 705, 704 }, // Silver Fishing Rod+ uses Silver Fishing Rod
            { 706, 705 }, // Golden Fishing Rod uses Silver Fishing Rod+
            { 707, 706 }, // Golden Fishing Rod+ uses Golden Fishing Rod
            { 708, 707 }, // Orichalcum Fishing Rod uses Golden Fishing Rod+
            { 710, 708 }, // Ultimate Fishing Rod uses Orichalcum Fishing Rod
        };
        
        private static readonly HashSet<uint> UpgradeCropExclusions = new HashSet<uint>
        {
            104000, 104001, 104002, 104003, 104004, 104005, 104006, 104007, 104008, 104009, 104010, 104011, 104012, 104013, 104014, 104015, 104016, 104017, 104018, 104019, 104020, 104021, 104022, 104023, 104024, 104025, 104026, 104027, 104028, 104100, 104101, 104102, 104103, 104104, 104105, 104106, 104107, 104108, 104109, 104110, 104111, 104112, 104113, 104114, 104203, 104204, 104205, 104207, 104208, 104209, 104210, 104212, 104213, 104214, 104215, 104216, 104217, 104218, 104219, 104300, 104301, 104302, 104303, 104305, 104306, 104307, 104308, 111100, 111101, 111102, 111103, 111104, 111105
        };

        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void AllCostOneItem()
        {
            if (!CheaperRecipesToggle.Value) return;

            var windCraftData = MasterDataManager.Instance.WindmillCraftingMasterData;

            foreach (var item in windCraftData)
            {
                for (var i = 0; i < item.RequiredItemId.Count; i++)
                {
                    if (CustomFirstItemByCraftedItem.TryGetValue(item.CraftingItemId, out var customFirstItemId))
                    {
                        // Exception logic: first = mapped item, second = CheaperRecipeItem.Value
                        if (i == 0)
                        {
                            item.RequiredItemId[i] = customFirstItemId;
                            item.RequiredItemStack[i] = 1;
                            item.RequiredItemType[i] = 0;
                        }
                        else if (i == 1)
                        {
                            item.RequiredItemId[i] = (uint)CheaperRecipeItem.Value;
                            item.RequiredItemStack[i] = 1;
                            item.RequiredItemType[i] = 0;
                        }
                        else
                        {
                            item.RequiredItemId[i] = 0;
                            item.RequiredItemStack[i] = 0;
                            item.RequiredItemType[i] = 0;
                        }
                    }
                    else
                    {
                        bool shouldApplyDefault =
                            !UpgradeCrops.Value || !UpgradeCropExclusions.Contains(item.CraftingItemId);

                        if (shouldApplyDefault)
                        {
                            item.RequiredItemId[i] = i == 0 ? (uint)CheaperRecipeItem.Value : 0;
                            item.RequiredItemStack[i] = i == 0 ? 1 : 0;
                            item.RequiredItemType[i] = 0;
                        }
                    }
                }
            }
        }
    }

    private static class CraftTimeAndHidden
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void FasterAndVisible()
        {
            var windCraftData = MasterDataManager.Instance.WindmillCraftingMasterData;
            
            if (CraftTimeToggle.Value)
            {
                foreach (var item in windCraftData)
                {
                    for (var i = 0; i < item.RequiredItemId.Count; i++)
                    {
                        item.Time = CraftTime.Value;
                    }
                }
            }

            if (HiddenItems.Value)
            {
                foreach (var item in windCraftData)
                {
                    for (var i = 0; i < item.RequiredItemId.Count; i++)
                    {
                        if (item.IsHidden)
                        {
                            item.IsHidden = false;
                        }
                    }
                }
            }
        }
    }

    private static class GameSettingStuff
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        
        private static void ChangingGameSettings()
        {
            GameController.Instance.GameSetting.WindmillSettings.CRAFT_LINE_MAX_1 = CraftSlots1.Value;
            GameController.Instance.GameSetting.WindmillSettings.CRAFT_LINE_MAX_2 = CraftSlots2.Value;
            GameController.Instance.GameSetting.WindmillSettings.MAX_MATERIAL_CNT_1 = CraftSize1.Value;
            GameController.Instance.GameSetting.WindmillSettings.MAX_MATERIAL_CNT_2 = CraftSize2.Value;
            GameController.Instance.GameSetting.WindmillSettings.WIND_BUFF[0] = WindStrength0.Value;
            GameController.Instance.GameSetting.WindmillSettings.WIND_BUFF[1] = WindStrength1.Value;
            GameController.Instance.GameSetting.WindmillSettings.WIND_BUFF[2] = WindStrength2.Value;
            GameController.Instance.GameSetting.WindmillSettings.WIND_BUFF[3] = WindStrength3.Value;
            GameController.Instance.GameSetting.WindmillSettings.NICE_COUNT_BUFF_COUNT = YellowCount.Value;
            GameController.Instance.GameSetting.WindmillSettings.NICE_COUNT_BUFF_INTERVAL = YellowInterval.Value;
            GameController.Instance.GameSetting.WindmillSettings.NICE_QUALITY_BUFF = RedQuality.Value;
            GameController.Instance.GameSetting.WindmillSettings.NICE_TIME_BUFF = BlueTime.Value;
        }
    }
}
