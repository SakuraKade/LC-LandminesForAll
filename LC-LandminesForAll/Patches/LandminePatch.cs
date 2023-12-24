using HarmonyLib;
using LC_LandminesForAll.Utils;
using UnityEngine;

namespace LC_LandminesForAll.Patches
{
    [HarmonyPatch(typeof(Landmine))]
    internal class LandminePatch
    {
        private static bool enemyOnMine = false;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool Update(Landmine __instance)
        {
            if (__instance.hasExploded)
                return true;
            
            if (enemyOnMine && !HasEnemiesOnMine(__instance))
            {
                enemyOnMine = false;
                ReflectionUtils.InvokePrivateMethod(__instance, "TriggerMineOnLocalClientByExiting");
                return true;
            }

            if (HasEnemiesOnMine(__instance))
            {
                enemyOnMine = true;
                const float debounceTime = 0.5f;
                ReflectionUtils.SetPrivateField(__instance, "pressMineDebounceTimer", debounceTime);
                __instance.PressMineServerRpc();
                return true;
            }

            return true;
        }

        private static bool HasEnemiesOnMine(Landmine __instance)
        {
            RaycastHit[] sphereCast = Physics.SphereCastAll(__instance.transform.position, 1f, Vector3.up, 0f);
            foreach (var hit in sphereCast)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    return true;
                }
            }
            return false;
        }

        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            if (other.CompareTag("Enemy"))
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

            if (other.CompareTag("Enemy"))
            {
                ReflectionUtils.InvokePrivateMethod(__instance, "TriggerMineOnLocalClientByExiting");
                return false;
            }

            return true;
        }
    }
}
