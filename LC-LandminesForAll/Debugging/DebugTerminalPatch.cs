#if DEBUG
using HarmonyLib;

namespace LC_LandminesForAll.Debugging
{
    [HarmonyPatch(typeof(Terminal))]
    internal class DebugTerminalPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(Terminal __instance)
        {
            __instance.groupCredits = 999999;
            __instance.SyncGroupCreditsServerRpc(__instance.groupCredits, __instance.numberOfItemsInDropship);
        }
    }
}
#endif