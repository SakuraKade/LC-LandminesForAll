using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LC_LandminesForAll.Debugging;
using LC_LandminesForAll.Patches;
using System.IO;

namespace LC_LandminesForAll
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;
        internal static new ManualLogSource Logger;
        public static string ThisPluginFolder => Path.Combine(Paths.PluginPath, "Sakura-LandminesForAll");

#if DEBUG
        private DebugLoggerRedirect _debugLoggerRedirect;
#endif

        private void Awake()
        {
            Logger = base.Logger;

#if DEBUG
            // Start the debug logger redirect first.
            _debugLoggerRedirect = new DebugLoggerRedirect(base.Logger);
#endif

            // Patches
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll(typeof(LandminePatch));

            // Debugging stuff
#if DEBUG
            _harmony.PatchAll(typeof(DebugPlayerControllerBPatch));
            _harmony.PatchAll(typeof(DebugLandminePatch));
            _harmony.PatchAll(typeof(DebugTerminalPatch));
#endif

            // Log done
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");         
        }
    }
}