using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Configuration;
using BokuMono;
using HarmonyLib;

namespace CameraZoom;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    private static ConfigEntry<float> ZoomLevel;

    public override void Load()
    {
        ZoomLevel = Config.Bind("Camera", "ZoomLevel", 21f, "Edit the camera distance. Default: 13");
        
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(CameraZoomPatch));
    }
    
    private class CameraZoomPatch {
        [HarmonyPatch(typeof(FollowingCamera))]
        [HarmonyPatch("get_FCS")]
        [HarmonyPostfix]

        public static void CameraZoom(FollowingCamera __instance)
        {
            __instance.currentDistance = ZoomLevel.Value;
        }
    }
}