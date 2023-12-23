using HarmonyLib;
using LC_LandminesForAll.Utils;
using UnityEngine;

namespace LC_LandminesForAll.Patches
{
    [HarmonyPatch(typeof(Landmine))]
    internal class LandminePatch
    {
        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            if (other.TryGetComponent<EnemyAI>(out _))
            {
                const float debounceTime = 0.5f;
                ReflectionUtils.SetPrivateField(__instance, "pressMineDebounceTimer", debounceTime);
                __instance.PressMineServerRpc();
                return false;
            }

            return true;
        }

        [HarmonyPatch("OnTriggerExit")]
        [HarmonyPrefix]
        private static bool OnTriggerExit(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            if (other.TryGetComponent<EnemyAI>(out _))
            {
                ReflectionUtils.InvokePrivateMethod(__instance, "TriggerMineOnLocalClientByExiting");
                return false;
            }

            return true;
        }
    }
}
