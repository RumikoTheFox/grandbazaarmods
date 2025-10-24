using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils;
using BokuMono;
using HarmonyLib;
using UnityEngine;

namespace RegenWhileSitting;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    private static ConfigEntry<float> benchTime;
    private static ConfigEntry<float> regenPercent;

    public override void Load()
    {
        benchTime = Config.Bind("--------01 BENCH SPEED--------", "Bench Speed",
            30f,
            "Set the flow of time when sitting. Game's default speed is 60.");
        regenPercent = Config.Bind("--------02 REGEN PERCENTAGE--------", "Regen Percentage",
            0.5f,
            "Set the amount of HP regen per second while sitting on a bench. Default is 0.5% of max HP per second.");
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(RegenControl));
    }

    public static class RegenControl
    {
        private static Coroutine _healPlayerCoroutine;
        private static float _timescale;
        private static float _regenAmount = regenPercent.Value / 100f;
        
        [HarmonyPatch(typeof(HumanActionSit), "Action")]
        [HarmonyPostfix]
        private static void BeginSit()
        {
            _timescale = DateManager.Instance.TimeScale;
            DateManager.Instance.TimeScale = benchTime.Value;
            _healPlayerCoroutine ??= GameController.Instance.StartCoroutine(HealPlayerHealth(2f));
        }
        
        [HarmonyPatch(typeof(HumanActionSit), "RemoveChair")]
        [HarmonyPostfix]
        private static void EndSit(FieldChair __result)
        {
            DateManager.Instance.TimeScale = _timescale;
            if (_healPlayerCoroutine == null) return;
            GameController.Instance.StopCoroutine(_healPlayerCoroutine);
            _healPlayerCoroutine = null;
        }
        
        private static System.Collections.Generic.IEnumerator<WaitForSeconds> HealPlayerHealth(float waitTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
                // Add regenAmount of max HP every waitTime seconds
                if(GameController.Instance.PlayerMaxHP * _regenAmount + GameController.Instance.PlayerHP > GameController.Instance.PlayerMaxHP)
                {
                    GameController.Instance.PlayerHP = GameController.Instance.PlayerMaxHP;
                }
                else
                {
                    GameController.Instance.PlayerHP += GameController.Instance.PlayerMaxHP * _regenAmount;
                }
            }
        }
    }
}