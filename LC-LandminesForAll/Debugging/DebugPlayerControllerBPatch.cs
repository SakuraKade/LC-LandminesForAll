#if DEBUG
#define DEBUG_GOD_MODE // Uncomment to make player invincible
#define DEBUG_SPEED_BOOST // Uncomment to make player faster
//#define DEBUG_PLAYER_IS_ENEMY // Uncomment to make player an enemy
#define DEBUG_SPAWN_LANDMINE_AT_WILL // Uncomment to spawn a landmine at where the player is looking at when pressing the "L" key
#define DEBUG_PLAYER_GLOW // Uncomment to make player glow
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LC_LandminesForAll.Debugging
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class DebugPlayerControllerBPatch
    {
        [HarmonyPatch("AllowPlayerDeath")]
        [HarmonyPrefix]
        private static bool AllowPlayerDeath(PlayerControllerB __instance, ref bool __result)
        {
#if DEBUG_GOD_MODE
            return false;
#else
            return true;
#endif
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(PlayerControllerB __instance)
        {
#if DEBUG_SPEED_BOOST
            __instance.movementSpeed *= 2.5f;
#endif
#if DEBUG_PLAYER_IS_ENEMY
            // Set tag to enemy for this object and all children
            __instance.gameObject.tag = "Enemy";
            Stack<GameObject> gameObjectsToSearch = new Stack<GameObject>();
            gameObjectsToSearch.Push(__instance.gameObject);
            while (gameObjectsToSearch.Count > 0)
            {
                GameObject gameObject = gameObjectsToSearch.Pop();
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.tag = "Enemy";
                    gameObjectsToSearch.Push(child.gameObject);
                }
            }
#endif
#if DEBUG_SPAWN_LANDMINE_AT_WILL
            InputActionMap inputActionMap = new InputActionMap();
            inputActionMap.AddAction("SpawnLandmine", binding: "<Keyboard>/l");
            inputActionMap.Enable();

            // Find the prefab for the landmine in the asset database at runtime
            GameObject landminePrefab = null;
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == "Landmine")
                {
                    landminePrefab = gameObject;
                    break;
                }
            }

            if (landminePrefab == null)
            {
                Debug.LogError("Landmine prefab not found");
                goto endOfSpawnLandmineAtWill;
            }

            inputActionMap["SpawnLandmine"].performed += (InputAction.CallbackContext context) =>
            {
                if (context.performed)
                {
                    // Spawn a landmine at where the player is looking at
                    Vector3 spawnPosition = __instance.transform.position + __instance.transform.forward * 2.5f;
                    GameObject landmine = Object.Instantiate(landminePrefab, spawnPosition, Quaternion.identity);
                    landmine.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    landmine.transform.eulerAngles = new Vector3(270f, 182f, 0f);
                    // Rotation: (270.00, 182.00, 0.00)
                }
            };
        endOfSpawnLandmineAtWill:;
#endif
#if DEBUG_PLAYER_GLOW
            GameObject playerGlow = new GameObject("PlayerGlow");
            playerGlow.transform.SetParent(__instance.transform);
            playerGlow.transform.localPosition = Vector3.zero + Vector3.up;
            playerGlow.transform.localRotation = Quaternion.identity;
            Light playerGlowLight = playerGlow.AddComponent<Light>();
            playerGlowLight.color = Color.white;
            playerGlowLight.intensity = 25f;
            playerGlowLight.range = 10f;
            playerGlowLight.type = LightType.Point;
            playerGlowLight.shadows = LightShadows.None;
            playerGlowLight.enabled = true;
#endif
        }
    }
}
#endif