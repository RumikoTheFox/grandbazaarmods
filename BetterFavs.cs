using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using System.Collections.Generic;
using BokuMono;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BetterFavs;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(ResizeFont));
        Harmony.CreateAndPatchAll(typeof(ReplaceFavorites));
    }

    // Reduce font size to fit new text
    private static class ResizeFont
    {
        [HarmonyPatch(typeof(UIBagMenuPage), "InitPage")]
        [HarmonyPostfix]
        private static void OnBagMenuInit(UIBagMenuPage __instance)
        {
            if (__instance == null) return;

            var residentDetail =
                Object.FindFirstObjectByType<UIResidentInfoDetail>(FindObjectsInactive.Include);

            if(residentDetail == null) return;
            //32 is default font size
            residentDetail.caption.fontSize = 24;
        }
    }

    private static class ReplaceFavorites
    {
        [HarmonyPatch(typeof(UITitleMainPage), "PlayTitleLogoAnimation")]
        [HarmonyPostfix]
        private static void FavReplacement()
        {
            var curLang = LanguageManager.Instance.CurrentLanguage;
            switch (curLang)
            {
                case Language.cs:
                    Log.LogWarning("很抱歉，此修改目前仅适用于英语。");
                    break;
                case Language.ct:
                    Log.LogWarning("很抱歉，此修改目前僅適用於英語。");
                    break;
                case Language.fr:
                    Log.LogWarning("Désolé, cette modification est actuellement disponible uniquement en anglais.");
                    break;
                case Language.ge:
                    Log.LogWarning("Entschuldigung, diese Änderung ist derzeit nur auf Englisch verfügbar.");
                    break;
                case Language.ja:
                    Log.LogWarning("申し訳ありませんが、この変更は現在英語のみで利用可能です。");
                    break;
                case Language.sp:
                    Log.LogWarning("Lo siento, esta modificación solo está disponible en inglés por el momento.");
                    break;
            }

            Dictionary<uint, string> profileTextOverrides = new Dictionary<uint, string>
            {
                // Jules
                { 10001, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Herbal Perfume\n<color=1>Likes:</color> <color=0>perfumes</color>, <color=0>herbs</color>, <color=0>salads</color>, Macaroni Salad, Broccoli & Garlic Sauté, <color=0>flowers</color>, <color=0>vegetables</color>, <color=0>milk</color>, <color=0>cheese</color>, <color=0>yogurt</color>, <color=0>pickled foods</color>, <color=0>dragonflies</color>, <color=0>tofu dishes</color>" },
                
                // Derek
                { 10101, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Cream Croquettes\n<color=1>Likes:</color> <color=0>horned beetles</color>, Boiled Tofu, Unadon, Donburi Rice Bowl, Farmer's Breakfast, Tempura Rice Bowl, Milk Hot Pot, Paella, <color=0>stag beetles</color>, <color=0>treasures</color>, <color=0>tea leaves</color>, <color=0>basic teas & tins (except Black)</color>, <color=0>main dish fondues</color>, <color=0>curry</color>, <color=0>whole cakes</color>" },
                
                // Lloyd
                { 10201, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Paella\n<color=1>Likes:</color> <color=0>high-end eggs</color>, Gold Medal, <color=0>diamonds</color>, Ancient Fish Fossil, Legendary Treasure Chest, Bouillabaisse, <color=0>main dish fondues</color>, Sushi, Meunière Fish, <color=0>high-end curries</color>, <color=0>tea leaves</color>, <color=0>fruit teas & tins</color>, <color=0>seasonal teas & tins</color>, <color=0>jewelry</color>, <color=0>insects</color>, <color=0>juice</color>" },
                
                // Gabriel
                { 10301, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Omelet Rice\n<color=1>Likes:</color> <color=0>pumpkin dishes</color>, French Fries, <color=0>croquettes</color>, Gratin, Pizza, Chirashi Sushi, Curry Rice, Milk Curry, Okonomiyaki, Chestnut Rice, Cheesecake, Ice Cream, Cake, Baumkuchen, <color=0>fruits</color>, <color=0>milk</color>, <color=0>yogurt</color>, <color=0>honey</color>, <color=0>fruit teas & tins</color>, <color=0>fruit jams & juice</color>, <color=0>desserts</color>, <color=0>curry dishes</color>" },
                
                // Samir
                { 10401, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Stew\n<color=1>Likes:</color> <color=0>eggplant dishes</color>, Steamed Egg Custard, Stir-Fried Vegetables, Babaocai, Boiled Tofu, Simmered Fish, Rice & Mixed Vegetables, Fried Udon, Yakisoba, <color=0>chocolate desserts</color>, Cooked Rice, <color=0>tea leaves</color>, <color=0>milk</color>, <color=0>stag beetles</color>, <color=0>porridge</color>" },
                
                // Arata
                { 10501, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Unadon\n<color=1>Likes:</color> <color=0>high-end milk</color>, Donburi Rice Bowl, Tempura Rice Bowl, Tempura Udon, Fried Udon, Tempura Soba, Yakisoba, Warm Milk, <color=0>milk</color>, <color=0>mushrooms</color>, <color=0>dragonflies</color>, <color=0>fish & bait</color>" },
                
                // Sophie
                { 20001, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Herb Salad\n<color=1>Likes:</color> <color=0>pickled foods</color>, <color=0>salads</color>, Macaroni Salad, Pasta Salad, <color=0>tofu dishes</color>, Rolled Egg, <color=0>porridge</color>, <color=0>basic teas & tins (except Black)</color>, <color=0>necklaces</color>, <color=0>butterflies</color>, <color=0>sandwiches</color>" },
                
                // June
                { 20101, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Cherry Tea\n<color=1>Likes:</color> <color=0>high-end milk</color>, Cherry Tea Tin, <color=0>jewelry</color>, Cherry Juice, Royal Milk Tea, Jam Tea, Black Tea Tin, Black Tea, <color=0>fruit teas & tins</color>, <color=0>seasonal teas & tins</color>, Sugar, <color=0>yarn</color>, <color=0>frogs</color>, <color=0>desserts (except whole cakes)</color>, <color=0>fruits</color>" },
                
                // Freya
                { 20201, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Citrus Perfume\n<color=1>Likes:</color> <color=0>jewelry</color>, Amethyst, Emerald, Sandrose, Topaz, Peridot, Ruby, Moonstone, Diamond, Pink Diamond, <color=0>cheese dishes</color>, Pizza, Risotto, Rice Gratin, <color=0>milk</color>, <color=0>cheese</color>, <color=0>curry</color>" },
                
                // Maple
                { 20301, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Engadiner Nusstorte\n<color=1>Likes:</color> <color=0>desserts (except Strawberry Pie)</color>, <color=0>flowers</color>, <color=0>fruit teas & tins</color>, <color=0>brooches</color>, <color=0>honey</color>, <color=0>diamonds</color>, <color=0>fireflies</color>, <color=0>butterflies</color>, <color=0>fruit jams & juice</color>" },
                
                // Kagetsu
                { 20401, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Soy Milk Pudding\n<color=1>Likes:</color> Soy Milk, Tofu, Boiled\nTofu, Chilled Tofu, Tofu Skin, <color=0>crops</color>, <color=0>flowers</color>, <color=0>tea leaves</color>, <color=0>herbs</color>, Chestnut, Walnut, Matcha Tea, Sencha Tea, Pu'er Tea, Oolong Tea, Watermelon Tea, <color=0>porridge</color>" },
                
                // Diana
                { 20501, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Apple Pie\n<color=1>Likes:</color> Apple, Golden Apple, Meunière Fish, Spicy Curry, Baked Apple, Apple Juice, Apple Jam, <color=0>fruits</color>, <color=0>tea leaves</color>, Sugar, Black Tea Tin, Black Tea, <color=0>fruit teas & tins</color>, <color=0>honey</color>, <color=0>butterflies</color>, Cheese Fondue, Tomato Fondue, Pink Fondue, <color=0>curry</color>, <color=0>desserts (except whole cakes)</color>" },
                
                // Child (Toddler)
                { 70101, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Pudding\n<color=1>Likes:</color> <color=0>desserts</color>, <color=0>fruits</color>, Moondrop Flower, Pansy, Sunflower, Magic Red Flower, <color=0>wool</color>, <color=0>yarn</color>, <color=0>yogurt</color>, <color=0>honey</color>, <color=0>bottled nuts</color>, Chocolate, Mochi, <color=0>soup (except Miso)</color>, Boiled Egg, Poached Egg, Steamed Egg Custard, Fish Cake, Fruit Sandwich, Cooked Rice, Bread, White Bread, Warm Milk, Stew, Hot Pot, <color=0>porridge</color>" },
                
                // Child (Son)
                { 70201, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Ice Cream\n<color=1>Likes:</color> <color=0>treasures</color>, <color=0>desserts</color>, <color=0>fruits</color>, Moondrop Flower, Pansy, Sunflower, Magic Red Flower, <color=0>wool</color>, <color=0>yarn</color>, <color=0>milk</color>, <color=0>cheese</color>, <color=0>butter</color>, <color=0>yogurt</color>, <color=0>mayonnaise</color>, <color=0>fruit teas & tins</color>, <color=0>bottled nuts</color>, <color=0>honey</color>, <color=0>medals</color>, <color=0>ores</color>, <color=0>gems</color>, <color=0>horned beetles</color>, <color=0>butterflies</color>, Chocolate, Mochi, <color=0>soup</color>, <color=0>croquettes</color>, <color=0>porridge</color>, <color=0>fondue</color>, <color=0>curry (except Spicy)</color>" },
                
                // Child (Daughter)
                { 70301, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Ice Cream\n<color=1>Likes:</color> <color=0>treasures</color>, <color=0>desserts</color>, <color=0>fruits</color>, Moondrop Flower, Pansy, Sunflower, Magic Red Flower, <color=0>wool</color>, <color=0>yarn</color>, <color=0>milk</color>, <color=0>cheese</color>, <color=0>butter</color>, <color=0>yogurt</color>, <color=0>mayonnaise</color>, <color=0>fruit teas & tins</color>, <color=0>bottled nuts</color>, <color=0>honey</color>, <color=0>medals</color>, <color=0>ores</color>, <color=0>gems</color>, <color=0>horned beetles</color>, <color=0>butterflies</color>, Chocolate, Mochi, <color=0>soup</color>, <color=0>croquettes</color>, <color=0>porridge</color>, <color=0>fondue</color>, <color=0>curry (except Spicy)</color>" },
                
                // Felix
                { 40001, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Spicy Curry\n<color=1>Likes:</color> Bouillabaisse, Curry Bread, Curry Bun, Stew, Gratin, Rice Gratin, <color=0>high-end curries</color>, Hot Pot, Magic Red Flower, <color=0>cheese</color>, <color=0>horned beetles</color>, Ancient Fish Fossil, Legendary Treasure Chest, <color=0>most soups</color>, Cream Croquettes, Cheese Croquettes, <color=0>main dish fondues</color>, <color=0>curry</color>" },
                
                // Erik
                { 40101, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Spring Blend Tea\n<color=1>Likes:</color> Caprese Salad, Vichyssoise Soup, Bouillabaisse, Focaccia Bread, Quiche, Ultimate Curry, Supreme Curry, <color=0>juice</color>, Black Tea, <color=0>fruit teas</color>, <color=0>herbal teas</color>, <color=0>seasonal teas</color>, Milk Tea, Royal Milk Tea, Jam Tea, <color=0>tea leaves</color>, Black Tea Tin, <color=0>fruit tea tins</color>, <color=0>herbal tea tins</color>, <color=0>seasonal tea tins</color>, <color=0>perfume</color>, <color=0>salads</color>, <color=0>most soups</color>, <color=0>sandwiches</color>" },
                
                // Stuart
                { 40201, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Mont Blanc Cake\n<color=1>Likes:</color> <color=0>seasonal teas & tins</color>, Mochi, Pasta Salad, Herb Salad, Spaghetti Soup, Udon, Fried Udon, Zaru Soba, Yakisoba, Pasta, Herb Pasta, <color=0>chestnut dishes</color>, Dango, Toasted Mochi, Wrapped Rice Cakes, <color=0>milk tea</color>, Jam Tea, <color=0>fruits</color>, Moondrop Flower, <color=0>wool</color>, <color=0>pickled foods</color>, Black Tea Tin, Black Tea, <color=0>fruit teas & tins</color>, <color=0>grasshoppers</color>, Chocolate, <color=0>juice</color>" },
                
                // Sonia
                { 40301, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Fruit Dumplings\n<color=1>Likes:</color> <color=0>fruit teas & tins</color>, Mochi, Fruit Sandwich, <color=0>pie</color>, Dango, Toasted Mochi, Wrapped Rice Cakes, <color=0>juice</color>, Engadiner Nusstorte, <color=0>fruits</color>, <color=0>flowers</color>, <color=0>wool and yarn</color>, <color=0>pickled foods</color>, Black Tea Tin, Black Tea, <color=0>herbal teas & tins</color>, <color=0>seasonal teas & tins</color>, <color=0>eggplant dishes</color>, <color=0>porridge</color>" },
                
                // Madeleine
                { 40401, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Fruit Sandwich\n<color=1>Likes:</color> Vegetable Curry, Baked Apple, <color=0>pie</color>, Strawberry Mochi, Fruit Dumplings, Engadiner Nusstorte, Coffee Bean, <color=0>fruits</color>, <color=0>flowers</color>, <color=0>eggs</color>, <color=0>basic teas and tins (except Black)</color>, <color=0>bracelets</color>, <color=0>yarn</color>, <color=0>flour</color>, <color=0>perfume</color>, Chocolate" },
                
                // Mina
                { 40501, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Herb Pasta\n<color=1>Likes:</color> <color=0>cheese</color>, <color=0>butter</color>, Olive Tea Tin, <color=0>herbal teas & tins</color>, Herb Oil, Herbal Perfume, <color=0>herbs</color>, <color=0>herb dishes</color>, Spaghetti Soup, <color=0>fruits</color>, <color=0>flowers</color>, <color=0>tea leaves</color>, <color=0>yogurt</color>, Black Tea, Black Tea Tin, <color=0>fruit teas & tins</color>, <color=0>seasonal teas & tins</color>, <color=0>necklaces</color>, <color=0>perfume</color>, <color=0>diamonds</color>, Chocolate, <color=0>salads</color>, <color=0>sandwiches</color>, <color=0>fondues</color>, <color=0>desserts (except mochi-based)</color>, <color=0>juice</color>" },
                
                // Wilbur
                { 40601, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Steamed Mushrooms\n<color=1>Likes:</color> Miso Glazed Eggplant, Fried Eggplant, Fried Mushrooms, Fried Matsutake Mushrooms, Chilled Tofu, Spicy Curry, Zaru Soba, Happy Mushroom Fry, Magic Red Flower, <color=0>cheese</color>, <color=0>pickled foods</color>, Rock, Wood, <color=0>medals</color>, <color=0>mushrooms</color>, <color=0>stone & lumber</color>, <color=0>ores</color>, <color=0>gems</color>, <color=0>horned beetles</color>, <color=0>fish & bait</color>, Fish Bait Stock, <color=0>curry dishes</color>, Cooked Rice" },
                
                // Clara
                { 40701, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Pickled Broccoli\n<color=1>Likes:</color> <color=0>pickled foods (except Veggie Mix)</color>, Broccoli & Garlic Sauté, Poke, <color=0>crops</color>, Magic Blue Flower, <color=0>eggs</color>, <color=0>necklaces</color>, <color=0>bracelets</color>, <color=0>flour</color>, <color=0>honey</color>, <color=0>herbs</color>, <color=0>mushrooms</color>, Sandrose, Topaz, <color=0>diamonds</color>, <color=0>fish & bait</color>, Fish Bait Stock, <color=0>foodstuffs</color>, <color=0>salads</color>, <color=0>basic soups</color>, <color=0>egg dishes</color>, <color=0>sandwiches</color>, <color=0>croquettes</color>, <color=0>porridge</color>, <color=0>basic curries</color>" },
                
                // Kevin
                { 40801, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Giant Stag Beetle\n<color=1>Likes:</color> <color=0>high-end milk</color>, <color=0>insects & frogs</color>, <color=0>treasures</color>, French Fries, <color=0>croquettes</color>, Gratin, Pizza, Chirashi Sushi, <color=0>curry (except Spicy)</color>, Milk Hot Pot, <color=0>desserts</color>, <color=0>fruits</color>, Magic Red Flower, <color=0>milk</color>, <color=0>cheese</color>, <color=0>yogurt</color>, <color=0>honey</color>, <color=0>mayonnaise</color>, Rock, Wood, Weeds, <color=0>medals</color>, <color=0>stone & lumber</color>, <color=0>ores</color>, <color=0>gems</color>, <color=0>fish & bait</color>, Fish Bait Stock, <color=0>fishing junk</color>, Chocolate, <color=0>milk drinks</color>" },
                
                // Isaac
                { 40901, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Jam-Filled Bun\n<color=1>Likes:</color> <color=0>sandwiches</color>, Raisin Bread, Herb Bread, Toast, French Toast, Steamed Muffin, Herb Bread, <color=0>fruits</color>, Magic Red Flower, <color=0>honey</color>, Rock, Wood, <color=0>medals</color>, <color=0>stone</color>, Mithril, Adamantite, Orichalcum, <color=0>gems</color>, <color=0>cicadas</color>, <color=0>desserts</color>, <color=0>jam</color>" },
                
                // Nadine
                { 41001, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Milk +\n<color=1>Likes:</color> Buffalo Milk +, Floral Perfume, <color=0>basic soups</color>, Spaghetti Soup, <color=0>porridge</color>, Warm Milk, <color=0>coffee</color>, Broccoli & Garlic Sauté, <color=0>crops</color>, Moondrop Flower, <color=0>tea leaves</color>, <color=0>milk</color>, <color=0>basic eggs</color>, <color=0>cheese</color>, <color=0>butter</color>, <color=0>yogurt</color>, <color=0>mayonnaise</color>, <color=0>pickled foods</color>, <color=0>jewelry</color>, <color=0>yarn & wool</color>, <color=0>foodstuffs</color>, <color=0>flour</color>, <color=0>honey</color>, <color=0>herbs</color>, <color=0>fish & bait</color>, <color=0>mushrooms</color>, <color=0>gems</color>, <color=0>fireflies</color>, <color=0>salads</color>, <color=0>basic curry</color>, <color=0>desserts</color>" },
                
                // Sylvia
                { 41101, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Donuts\n<color=1>Likes:</color> <color=0>eggs</color>, <color=0>egg dishes</color>, <color=0>potato dishes</color>, Baked Sweet Potato, <color=0>croquettes</color>, Curry Bun, Cheese Bun, Gyoza, Focaccia Bread, Quiche, Stew, Tempura, Galette, Steamed Bun, Gratin, Pizza, Chirashi Sushi, Curry Rice, Seaweed Curry, Milk Curry, Milk Hot Pot, Paella, <color=0>desserts</color>, Hot Pot, <color=0>fruits</color>, <color=0>jewelry</color>, <color=0>honey</color>, <color=0>medals</color>" },
                
                // Laurie
                { 41201, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Sweet Potato Cakes\n<color=1>Likes:</color> <color=0>milk</color>, <color=0>high-end wool</color>, Yogurt +, <color=0>potato dishes</color>, Baked Sweet Potato, <color=0>croquettes</color>, Curry Bun, Cheese Bun, Gyoza, Focaccia Bread, Quiche, Stew, Tempura, Galette, Steamed Bun, Omelet, Omelet Rice, Gratin, Pizza, Chirashi Sushi, Curry Rice, Seaweed Curry, Milk Curry, Milk Hot Pot, Paella, <color=0>desserts</color>, Hot Pot, <color=0>jewelry</color>, <color=0>perfume</color>, <color=0>herbs</color>" },
                
                // Miguel
                { 41301, "<color=1>Color:</color> Red\n<color=1>Fav:</color> Croquettes\n<color=1>Likes:</color> <color=0>high-end eggs</color>, <color=0>jewelry</color>, <color=0>fireflies</color>, Ancient Fish Fossil, Legendary Treasure Chest, Spice, French Fries, Curry Bread, <color=0>croquettes</color>, Focaccia Bread, Spicy Curry, <color=0>high-end curries</color>, Magic Red Flower, <color=0>medals</color>, <color=0>ores (except Iron)</color>, <color=0>gems</color>, <color=0>curry dishes</color>" },
                
                // Harold
                { 41401, "<color=1>Color:</color> Blue\n<color=1>Fav:</color> Ultimate Curry\n<color=1>Likes:</color> <color=0>high-end curries</color>, Pumpkin Pudding, Pudding, Soy Milk Pudding, Golden Blend Tea, <color=0>tea leaves</color>, Black Tea, Black Tea Tin, <color=0>fruit teas & tins</color>, <color=0>herbal teas & tins</color>, <color=0>seasonal teas & tins</color>, <color=0>beetles</color>, Spice, Curry Powder, <color=0>curry dishes</color>" },
                
                // Sherene
                { 41501, "<color=1>Color:</color> Yellow\n<color=1>Fav:</color> Seaweed Curry\n<color=1>Likes:</color> Chocolate, Simmered Seaweed, <color=0>chocolate desserts</color>, <color=0>jam</color>, Pet Food, Pet Treat, <color=0>fruits</color>, <color=0>flowers</color>, <color=0>high-end yogurt</color>, <color=0>jewelry</color>, <color=0>honey</color>, <color=0>perfume</color>, <color=0>fireflies</color>, <color=0>butterflies</color>, <color=0>desserts</color>, <color=0>juice</color>" },
                
                // Ivy (Harvest)
                { 50001, "Accepted Turn-Ins: Toy Flower, Moondrop Flower, Magic Blue Flower, Pink Cat Flower, Magic Red Flower, Rock, Wood, Chamomile, Mint, Lavender, Chestnut, Walnut, Weeds, Stone, Copper, Silver, Gold, Mithril, Adamantite, Orichalcum, Amethyst, Emerald, Sandrose, Topaz, Peridot, Fluorite, Agate, Ruby, Moonstone, Diamond, Pink Diamond" },
                
                // Finley (Fish)
                { 50101, "Accepted Turn-Ins: Any and every fish." },
                
                // Honey (Honey)
                { 50201, "Accepted Turn-Ins: Honey, Invigorating Honey, Mellow Honey, Royal Honey, Honeycomb, Invigorating Honeycomb, Mellow Honeycomb, Royal Honeycomb" },
                
                // Webby (Mushroom)
                { 50301, "Accepted Turn-Ins: Shiitake Mushroom, Shimeji Mushroom, Common Mushroom, Porcini Mushroom, Morel Mushroom, Matsutake Mushroom, Monarch Mushroom" },
                
                // Penny (Insects)
                { 50401, "Accepted Turn-Ins: Any and every insect." }
            };
            
            foreach (var kvp in profileTextOverrides)
            {
                uint id = kvp.Key;
                string newText = kvp.Value;

                var profileLocation = LanguageManager.Instance.GetLocalizeTextData(LocalizeTextTableType.CharacterCaptionText, id);
                if (profileLocation != null)
                {
                    profileLocation.Text = newText;
                }
            }
        }
    }
}