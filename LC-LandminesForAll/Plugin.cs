using BepInEx;
using HarmonyLib;

namespace LC_LandminesForAll
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;

        private void Awake()
        {
            // Patches
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            // Log done
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");         
        }
    }
}