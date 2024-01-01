using HarmonyLib;
using LC_LandminesForAll.Utils;
using System.Linq;
using UnityEngine;

namespace LC_LandminesForAll.Patches
{
    [HarmonyPatch(typeof(Landmine))]
    internal class LandminePatch
    {
        const int LineOfSightLayer = 18;
        const int EnemyLayer = 19;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(Landmine __instance)
        {
            // Copy the existing collider trigger
            BoxCollider mineTrigger = __instance.GetComponents<BoxCollider>().FirstOrDefault(x => x.isTrigger);
            if (mineTrigger == null)
            {
                Plugin.Logger.LogError($"Could not find mine trigger collider on mine at {__instance.transform.position}");
                return;
            }

            // Create a new collider trigger
            const float monsterTriggerSizeMultiplier = 2f;
            BoxCollider newTrigger = __instance.gameObject.AddComponent<BoxCollider>();
            newTrigger.isTrigger = true;
            newTrigger.size = mineTrigger.size * monsterTriggerSizeMultiplier;
            newTrigger.center = mineTrigger.center;
            newTrigger.enabled = true;
            newTrigger.includeLayers = 1 << EnemyLayer;
        }

        private static void TriggerDebounce(Landmine __instance)
        {
            Transform closestEnemy = GetClosestEnemyForLogging(__instance);
            Plugin.Logger.LogDebug($"Triggering debounce on mine at {__instance.transform.position} with closest enemy at {closestEnemy.position} at {Vector3.Distance(__instance.transform.position, closestEnemy.transform.position)} units away.");

            const float debounceTime = 0.5f;
            ReflectionUtils.SetPrivateField(__instance, "pressMineDebounceTimer", debounceTime);
            __instance.PressMineServerRpc();
        }

        private static void TriggerMineExplosion(Landmine __instance)
        {
            Transform closestEnemy = GetClosestEnemyForLogging(__instance);
            Plugin.Logger.LogDebug($"Triggering debounce on mine at {__instance.transform.position} with closest enemy at {closestEnemy.position} at {Vector3.Distance(__instance.transform.position, closestEnemy.transform.position)} units away.");

            ReflectionUtils.InvokePrivateMethod(__instance, "TriggerMineOnLocalClientByExiting");
        }

        /// <summary>
        /// Expensive method, only use for logging
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        private static Transform GetClosestEnemyForLogging(Landmine __instance)
        {
            LayerMask layerMask = 1 << EnemyLayer;
            RaycastHit[] sphereCast = Physics.SphereCastAll(__instance.transform.position, radius: 10000f, Vector3.up, maxDistance: 0f, layerMask);
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;
            foreach (var hit in sphereCast)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    float distance = Vector3.Distance(__instance.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = hit.transform;
                    }
                }
            }
            return closestEnemy;
        }

        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        private static bool OnTriggerEnter(Landmine __instance, Collider other)
        {
            if (__instance.hasExploded || ReflectionUtils.GetPrivateField<float>(__instance, "pressMineDebounceTimer") > 0f)
                return true;

            if (other.CompareTag("Enemy"))
            {
                TriggerDebounce(__instance);
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
                TriggerMineExplosion(__instance);
                return false;
            }

            return true;
        }
    }
}
