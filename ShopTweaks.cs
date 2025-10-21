using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;
using System.IO;
using System.Text.Json;
using BokuMono.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ShopTweaks;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    // Configuration entries
    private static ConfigEntry<bool> noShopConditions;
    private static ConfigEntry<bool> miguelRework;
    private static ConfigEntry<int> qualityControl;
    private static ConfigEntry<bool> infiniteStock;
    private static ConfigEntry<bool> recyclingConditions;
    private static ConfigEntry<bool> coroConditions;
    private static ConfigEntry<bool> expansionConditions;
    private static ConfigEntry<bool> basementException;
    private static ConfigEntry<bool> animalConditions;
    private static ConfigEntry<int> basementCost;

    public override void Load()
    {
        // Settings for removing conditions from shops
        noShopConditions = Config.Bind("--------01 CONDITION CONTROLS--------", "Allow All Items Without Conditions",
            false,
            "Allows most normal shop items to be purchased without meeting their conditions.");
        recyclingConditions = Config.Bind("--------01 CONDITION CONTROLS--------", "Include Recycling Shop",
            true,
            "If \"Allow All Items Without Conditions\" is true, setting this to true will also remove the conditions from the Recycling Shop.");
        coroConditions = Config.Bind("--------01 CONDITION CONTROLS--------", "Include Sprite Shop",
            true,
            "If \"Allow All Items Without Conditions\" is true, setting this to true will also remove the conditions from the Sprite Shop.");
        expansionConditions = Config.Bind("--------01 CONDITION CONTROLS--------", "Include Expansion Shops",
            true,
            "If \"Allow All Items Without Conditions\" is true, setting this to true will also remove the conditions from the Expansion Shops.\nNote: Conditions that require you to get a certain expansion first will NOT be removed to avoid breaking progression.");
        animalConditions = Config.Bind("--------01 CONDITION CONTROLS--------", "Include Animal Shop",
            true,
            "If \"Allow All Items Without Conditions\" is true, setting this to true will also remove the conditions from the Animal Shop.\nKeep in mind that this does not affect the RNG for which animals are being sold each week.\nIt only removes conditions like bazaar rank.");
        basementException = Config.Bind("--------01 CONDITION CONTROLS--------", "No Conditions for Basement", false, "Removes all conditions for purchasing a basement, including purchasing the other fields first.\nIf you use both this and \"Include Expansion Shops\", this toggle will take priority for the basement.\nDOES NOT REQUIRE \"ALLOW ALL ITEMS WITHOUT CONDITIONS\" TO BE ENABLED.");
        
        // Settings for other shop modifications
        infiniteStock = Config.Bind("--------02 SHOP MODIFICATIONS--------", "All Non Unique Stock Infinite",
            false,
            "Changes the stock of all non-unique items to be infinite");
        qualityControl = Config.Bind("--------02 SHOP MODIFICATIONS--------", "Quality Level for All Shop",
            1,
            "Sets the quality level of all quality-viable items in all shops. Each number is a half star,\nso a 5 is 2.5 stars, 8 is 4 stars, etc. Game Default is 1, or half a star. Max is 14, or 7 stars.");
        basementCost = Config.Bind("--------02 SHOP MODIFICATIONS--------", "Basement Cost", 1000000, "Lets you set the basement cost.");
        
        // Toggle setting for the rework of Miguel's shop
        miguelRework = Config.Bind("--------03 MIGUELS ULTIMATE MERCANTILE--------", "Allow Miguel Shop Rework",
            false,
            "Adds most non-clothing, non-expansion, non-animal items to Miguel's shop.\nAll options in Shop Modifications will affect Miguel's shop as well.");
        
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        // Harmony patches
        Harmony.CreateAndPatchAll(typeof(AddShopItems));
        Harmony.CreateAndPatchAll(typeof(ShopConditions));
        Harmony.CreateAndPatchAll(typeof(RecyclingShopConditions));
        Harmony.CreateAndPatchAll(typeof(CoroShopConditions));
        Harmony.CreateAndPatchAll(typeof(ExpansionShopConditions));
        Harmony.CreateAndPatchAll(typeof(FreeBasement));
        Harmony.CreateAndPatchAll(typeof(AnimalShopConditions));
        Harmony.CreateAndPatchAll(typeof(CustomQuality));
        Harmony.CreateAndPatchAll(typeof(InfiniteStockClass));
        Harmony.CreateAndPatchAll(typeof(BasementCost));
    }

    private static class BasementCost
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]

        public static void SetBasementCost()
        {
            MasterDataManager.Instance.ExpansionMaster.list[4].Price = basementCost.Value;
        }
    }

    // Class for removing conditions from the Animal Shop (ShopAnimalMaster)
    private static class AnimalShopConditions
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        public static void AnimalShopMethod()
        {
            if (!noShopConditions.Value || !animalConditions.Value) return;

            var animalList = MasterDataManager.Instance.ShopAnimalMaster.list;

            var typeListProp = typeof(ShopAnimalMasterData).GetProperty("ConditionsTypeList");
            var valueListProp = typeof(ShopAnimalMasterData).GetProperty("ConditionsValueList");

            if (typeListProp == null || valueListProp == null)
            {
                Log.LogWarning("[ANIMAL PATCH] Could not find ConditionsTypeList or ConditionsValueList via reflection.");
                return;
            }

            foreach (var item in animalList)
            {
                var rawTypeList = typeListProp.GetValue(item);
                var rawValueList = valueListProp.GetValue(item);

                if (rawTypeList == null || rawValueList == null)
                {
                    Log.LogWarning($"[ANIMAL PATCH] Conditions lists are null for item {item.Id}.");
                    continue;
                }

                var typeList = rawTypeList as Il2CppSystem.Collections.Generic.List<BokuMono.ConditionsType>;
                var valueList = rawValueList as Il2CppSystem.Collections.Generic.List<int>;

                if (typeList == null || valueList == null)
                {
                    Log.LogWarning($"[ANIMAL PATCH] Failed to cast condition lists for item {item.Id}.");
                    continue;
                }

                var newTypeList = new Il2CppSystem.Collections.Generic.List<BokuMono.ConditionsType>();
                var newValueList = new Il2CppSystem.Collections.Generic.List<int>();

                for (int i = 0; i < typeList.Count; i++)
                {
                    newTypeList.Add((BokuMono.ConditionsType)0);
                    newValueList.Add(0); // pad to preserve index alignment
                }

                typeListProp.SetValue(item, newTypeList);
                valueListProp.SetValue(item, newValueList);
            }

            Log.LogInfo("[ANIMAL PATCH] Cleared all conditions in ShopAnimalMaster.");
        }
    }
    
    // Class for removing conditions from the Expansion Shops (ShopExpansionMaster). Bypasses ConditionsType 17 to not break expansion progression.
    private static class ExpansionShopConditions
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        public static void ExpansionShopMethod()
        {
            if (!noShopConditions.Value || !expansionConditions.Value) return;

            var expansionList = MasterDataManager.Instance.ShopExpansionMaster.list;

            var typeListProp = typeof(ShopExpansionMasterData).GetProperty("ConditionsTypeList");
            var valueListProp = typeof(ShopExpansionMasterData).GetProperty("ConditionsValueList");

            if (typeListProp == null || valueListProp == null)
            {
                Log.LogWarning("[EXPANSION PATCH] Could not find ConditionsTypeList or ConditionsValueList via reflection.");
                return;
            }

            foreach (var item in expansionList)
            {
                var rawTypeList = typeListProp.GetValue(item);
                var rawValueList = valueListProp.GetValue(item);

                if (rawTypeList == null || rawValueList == null)
                {
                    Log.LogWarning($"[EXPANSION PATCH] Conditions lists are null for item {item.Id}.");
                    continue;
                }

                var typeList = rawTypeList as Il2CppSystem.Collections.Generic.List<BokuMono.ConditionsType>;
                var valueList = rawValueList as Il2CppSystem.Collections.Generic.List<int>;

                if (typeList == null || valueList == null)
                {
                    Log.LogWarning($"[EXPANSION PATCH] Failed to cast condition lists for item {item.Id}.");
                    continue;
                }

                var newTypeList = new Il2CppSystem.Collections.Generic.List<BokuMono.ConditionsType>();
                var newValueList = new Il2CppSystem.Collections.Generic.List<int>();

                for (int i = 0; i < typeList.Count; i++)
                {
                    var conditionType = typeList[i];
                    bool isBasement = item.Id == 100202 && basementException.Value;

                    if (isBasement)
                    {
                        newTypeList.Add(0);
                        newValueList.Add(0); // pad to preserve index alignment
                    }
                    else if ((int)conditionType == 17)
                    {
                        newTypeList.Add(conditionType);
                        newValueList.Add(i < valueList.Count ? valueList[i] : 0);
                    }
                    else
                    {
                        newTypeList.Add(0);
                        newValueList.Add(0); // pad with zero to preserve alignment
                    }
                }

                typeListProp.SetValue(item, newTypeList);
                valueListProp.SetValue(item, newValueList);
            }

            Log.LogInfo("[EXPANSION PATCH] Applied condition filtering to ShopExpansionMaster.");
        }
    }
    
    // Class for making the basement purchasable without conditions
    private static class FreeBasement
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        public static void FreeBasementMethod()
        {
            if (!basementException.Value) return;
            if (noShopConditions.Value && expansionConditions.Value) return; // Skip if the full expansion conditions patch is already active

            var expansionList = MasterDataManager.Instance.ShopExpansionMaster.list;

            var typeListProp = typeof(ShopExpansionMasterData).GetProperty("ConditionsTypeList");
            var valueListProp = typeof(ShopExpansionMasterData).GetProperty("ConditionsValueList");

            if (typeListProp == null || valueListProp == null)
            {
                Log.LogWarning("[BASEMENT PATCH] Could not find ConditionsTypeList or ConditionsValueList via reflection.");
                return;
            }

            foreach (var item in expansionList)
            {
                if (item.Id != 100202) continue; // Target only the basement item

                var typeList = typeListProp.GetValue(item) as List<int>;
                var valueList = valueListProp.GetValue(item) as List<int>;

                if (typeList == null || valueList == null || typeList.Count != valueList.Count)
                {
                    Log.LogWarning("[BASEMENT PATCH] Conditions lists are null or misaligned.");
                    continue;
                }

                for (int i = 0; i < typeList.Count; i++)
                {
                    typeList[i] = 0;
                    valueList[i] = 0;
                }

                Log.LogInfo("[BASEMENT PATCH] Cleared all conditions for basement item (Id: 100202).");
            }
        }
    }
    
    // Class for making all non-unique stock infinite
    private static class InfiniteStockClass
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void InfiniteStockMethod()
        {
            if (!infiniteStock.Value) return;

            var shopItemList = MasterDataManager.Instance.ShopItemMaster.list;
            var limitTypeProp = typeof(ShopItemMasterData).GetProperty("LimitType");
            var buyLimitProp = typeof(ShopItemMasterData).GetProperty("BuyLimit");
            
            if (limitTypeProp == null || buyLimitProp == null)
            {
                Log.LogWarning("Could not find LimitType or BuyLimit property via reflection.");
                return;
            }

            foreach (var shopitem in shopItemList)
            {
                var currentLimitType = (int)limitTypeProp.GetValue(shopitem);
                
                if (currentLimitType < 0 || currentLimitType > 2)
                {
                    Log.LogWarning($"[INFINITE STOCK] Invalid LimitType value detected: {currentLimitType}. Must be 0-2. Aborting infinite stock patch.");
                    return;
                }

                if (currentLimitType != 1)
                {
                    limitTypeProp.SetValue(shopitem, 0);
                    buyLimitProp.SetValue(shopitem, -1);
                }
            }
        }
    }
    
    // Class for setting all shop item qualities to qualityControl.Value
    private static class CustomQuality
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void CustomQualityMethod()
        {
            if (qualityControl.Value < 0 || qualityControl.Value > 14) return;

            var shopItemList = MasterDataManager.Instance.ShopItemMaster.list;

            // Cache the ambiguous Quality property
            var qualityProp = typeof(ShopItemMasterData).GetProperty("Quality");

            if (qualityProp == null)
            {
                Log.LogWarning("Could not find Quality property via reflection.");
                return;
            }

            foreach (var shopitem in shopItemList)
            {
                // Read current quality
                var currentQuality = (int)qualityProp.GetValue(shopitem);

                // Only override if it's above 0
                if (currentQuality > 0)
                {
                    qualityProp.SetValue(shopitem, qualityControl.Value);
                }
            }
        }
    }
    
    // Class for removing conditions from the normal shops (ShopItemMaster)
    private static class ShopConditions
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void ShopConditionsMethod()
        {
            if (!noShopConditions.Value) return;

            var shopItemList = MasterDataManager.Instance.ShopItemMaster.list;

            foreach (var shopitem in shopItemList)
            {
                SetAllSeasonsAvailable(shopitem);
                ZeroOutIntListProperty(shopitem, "ConditionsTypeList");
                ZeroOutIntListProperty(shopitem, "ConditionsValueList");
                ZeroOutIntListProperty(shopitem, "ConditionsValueList2");
            }
        }

        private static void SetAllSeasonsAvailable(object shopitem)
        {
            var seasonList = shopitem.GetType().GetProperty("SeasonList")?.GetValue(shopitem) as Il2CppSystem.Collections.Generic.List<int>;
            if (seasonList != null)
            {
                for (int i = 0; i < seasonList.Count; i++)
                {
                    seasonList[i] = 1;
                }
            }
        }

        private static void ZeroOutIntListProperty(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop?.CanRead == true)
            {
                var list = prop.GetValue(obj) as Il2CppSystem.Collections.Generic.List<int>;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = 0;
                    }
                }
            }
        }
    }
    
    // Class for removing conditions from the Sprite Shop (ShopCoroMaster)
    private static class CoroShopConditions
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void CoroShopConditionsMethod()
        {
            if (!noShopConditions.Value || !coroConditions.Value) return;

            var shopItemList = MasterDataManager.Instance.ShopCoroMaster.list;

            foreach (var shopitem in shopItemList)
            {
                SetAllSeasonsAvailable(shopitem);
                ZeroOutIntListProperty(shopitem, "ConditionsTypeList");
                ZeroOutIntListProperty(shopitem, "ConditionsValueList");
                ZeroOutIntListProperty(shopitem, "ConditionsValueList2");
            }
        }

        private static void SetAllSeasonsAvailable(object shopitem)
        {
            var seasonList = shopitem.GetType().GetProperty("SeasonList")?.GetValue(shopitem) as Il2CppSystem.Collections.Generic.List<int>;
            if (seasonList != null)
            {
                for (int i = 0; i < seasonList.Count; i++)
                {
                    seasonList[i] = 1;
                }
            }
        }

        private static void ZeroOutIntListProperty(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop?.CanRead == true)
            {
                var list = prop.GetValue(obj) as Il2CppSystem.Collections.Generic.List<int>;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = 0;
                    }
                }
            }
        }
    }
    
    // Class for removing conditions from the Recycling Shop (ShopRecyclingMaster)
    private static class RecyclingShopConditions
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void RecyclingShopMethod()
        {
            if (!noShopConditions.Value || !recyclingConditions.Value) return;
            
            var recyclingList = MasterDataManager.Instance.ShopRecyclingMaster.list;

            if (recyclingList == null || recyclingList.Count == 0)
            {
                Log.LogWarning("[SHOP TWEAKS] Recycling list is empty or null.");
                return;
            }

            var targetItem = recyclingList[0];

            var typeListProp = typeof(ShopRecyclingMasterData).GetProperty("ConditionsTypeList");
            var valueListProp = typeof(ShopRecyclingMasterData).GetProperty("ConditionsValueList");

            if (typeListProp == null || valueListProp == null)
            {
                Log.LogWarning("[SHOP TWEAKS] Could not find ConditionsTypeList or ConditionsValueList via reflection.");
                return;
            }

            var typeList = typeListProp.GetValue(targetItem) as List<int>;
            var valueList = valueListProp.GetValue(targetItem) as List<int>;

            if (typeList == null || valueList == null || typeList.Count == 0 || valueList.Count == 0)
            {
                Log.LogWarning("[SHOP TWEAKS] Conditions lists are null or empty.");
                return;
            }

            typeList[0] = 0;
            valueList[0] = 0;

            Log.LogInfo("[SHOP TWEAKS] ConditionsTypeList[0] and ConditionsValueList[0] set to 0.");
        }
    }
    
    // Class for adding new items to Miguel's shop from an external JSON file
    private static class AddShopItems
    {
        // Cache PropertyInfo for ProductCategory to avoid ambiguity issues
        private static readonly PropertyInfo ProductCategoryProp =
            typeof(ShopItemMasterData).GetProperty("ProductCategory");
        private static readonly PropertyInfo QualityProp =
            typeof(ShopItemMasterData).GetProperty("Quality");
        private static readonly PropertyInfo LimitTypeProp =
            typeof(ShopItemMasterData).GetProperty("LimitType");
        private static readonly PropertyInfo PriorityProp =
            typeof(ShopItemMasterData).GetProperty("Priority");
        private static readonly PropertyInfo ConditionsTypeListProp =
            typeof(ShopItemMasterData).GetProperty("ConditionsTypeList");
        private static readonly PropertyInfo ConditionsValueListProp =
            typeof(ShopItemMasterData).GetProperty("ConditionsValueList");

        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void AddShopItemsMethod()
        {
            if (!miguelRework.Value) return;

            var shopStockList = MasterDataManager.Instance.ShopItemMaster.list;

            // Step 1: Remove all items with Category == ShopCategory.Shop01
            for (int i = shopStockList.Count - 1; i >= 0; i--)
            {
                if (shopStockList[i].Category == ShopCategory.Shop01)
                    shopStockList.RemoveAt(i);
            }

            // Step 2: Load JSON from BepInEx/plugins
            string jsonPath = Path.Combine(Paths.PluginPath, "ShopItems.json");

            if (!File.Exists(jsonPath))
            {
                Log.LogInfo($"ShopItems.json not found at {jsonPath}");
                return;
            }

            string rawJson = File.ReadAllText(jsonPath);
            // Below code allows comments in JSON
            string cleanJson = Regex.Replace(rawJson, @"//.*?$|/\*.*?\*/", "", RegexOptions.Singleline | RegexOptions.Multiline);
            var templates = JsonSerializer.Deserialize<List<ShopItemTemplate>>(cleanJson);

            if (templates == null)
            {
                Log.LogInfo("Failed to deserialize ShopItems.json");
                return;
            }

            // Step 3: Inject new items
            foreach (var template in templates)
            {
                var newItem = new ShopItemMasterData
                {
                    Id = template.Id,
                    Category = (ShopCategory)template.Category,
                    ItemId = template.ItemId,
                    BuyLimit = template.BuyLimit,
                    ScreenshotId = template.ScreenshotId,
                    Weight = template.Weight
                };

                // Assign ProductCategory via cached reflection
                if (ProductCategoryProp != null && ProductCategoryProp.CanWrite)
                    ProductCategoryProp.SetValue(newItem, (ShopProductCategory)template.ProductCategory);

                if (QualityProp != null) QualityProp.SetValue(newItem, template.Quality);
                if (LimitTypeProp != null) LimitTypeProp.SetValue(newItem, template.LimitType);
                if (PriorityProp != null) PriorityProp.SetValue(newItem, template.Priority);

                // Manually populate Il2Cpp lists to avoid constructor ambiguity
                var seasonList = new Il2CppSystem.Collections.Generic.List<int>();
                foreach (var value in template.SeasonList)
                    seasonList.Add(value);
                newItem.SeasonList = seasonList;

                var conditionsTypeList = new Il2CppSystem.Collections.Generic.List<ConditionsType>();
                foreach (var value in template.ConditionsTypeList)
                {
                    conditionsTypeList.Add((ConditionsType)value);
                }
                if (ConditionsTypeListProp != null)
                    ConditionsTypeListProp.SetValue(newItem, conditionsTypeList);

                var conditionsValueList = new Il2CppSystem.Collections.Generic.List<int>();
                foreach (var value in template.ConditionsValueList)
                    conditionsValueList.Add(value);
                if (ConditionsValueListProp != null)
                    ConditionsValueListProp.SetValue(newItem, conditionsValueList);

                var conditionsValueList2 = new Il2CppSystem.Collections.Generic.List<int>();
                foreach (var value in template.ConditionsValueList2)
                    conditionsValueList2.Add(value);
                newItem.ConditionsValueList2 = conditionsValueList2;

                shopStockList.Add(newItem);
            }
        }

        private class ShopItemTemplate
        {
            public uint Id { get; set; }
            public int Category { get; set; }
            public int ProductCategory { get; set; }
            public uint ItemId { get; set; }
            public int Quality { get; set; }
            public int LimitType { get; set; }
            public int BuyLimit { get; set; }
            public string ScreenshotId { get; set; }
            public int Priority { get; set; }
            public int Weight { get; set; }
            public List<int> SeasonList { get; set; }
            public List<int> ConditionsTypeList { get; set; }
            public List<int> ConditionsValueList { get; set; }
            public List<int> ConditionsValueList2 { get; set; }
        }
    }
}