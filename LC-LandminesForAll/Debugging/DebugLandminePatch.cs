#if DEBUG
using HarmonyLib;
using LC_LandminesForAll.Utils;
using UnityEngine;

namespace LC_LandminesForAll.Debugging
{
    [HarmonyPatch(typeof(Landmine))]
    internal class DebugLandminePatch
    {
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            // Ignore player
            if (other.CompareTag("Player"))
                return false;

            return true;
        }

        [HarmonyPatch("OnTriggerExit")]
        [HarmonyPrefix]
        private static bool OnTriggerExit(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            // Ignore player
            if (other.CompareTag("Player"))
                return false;

            return true;
        }
    }
}
#endif