#if DEBUG
using GameNetcodeStuff;
using HarmonyLib;

namespace LC_LandminesForAll.Debugging
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class DebugPlayerControllerBPatch
    {
        [HarmonyPatch("AllowPlayerDeath")]
        [HarmonyPrefix]
        private static bool AllowPlayerDeath(PlayerControllerB __instance, ref bool __result)
        {
            return false;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(PlayerControllerB __instance)
        {
            __instance.movementSpeed *= 2.5f;
        }
    }
}
#endif