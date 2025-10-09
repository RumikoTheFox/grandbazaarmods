using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BokuMono;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ABC_Cookbook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(ABCCookbookClass));
    }

    private static class ABCCookbookClass
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void SortCookbookAlphabetically()
        {
            var cookingList = MasterDataManager.Instance?.CookingMaster?.list?.ToArray();
            if (cookingList == null)
            {
                Log.LogWarning("CookingMaster list is null. Cannot sort cookbook.");
                return;
            }

            var sorted = cookingList
                .Select(recipe => new {
                    Recipe = recipe,
                    Text = LanguageManager.Instance.GetLocalizeTextData(LocalizeTextTableType.ItemNameText, recipe.Id)?.Text
                })
                .Where(x => !string.IsNullOrEmpty(x.Text))
                .OrderBy(x => x.Text, System.StringComparer.OrdinalIgnoreCase)
                .ToList();

            int startPriority = 30000000;

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].Recipe.Priority = startPriority - i;
            }

            Log.LogInfo($"Cookbook sorted alphabetically by localized name. {sorted.Count} recipes updated.");
        }
    }
}