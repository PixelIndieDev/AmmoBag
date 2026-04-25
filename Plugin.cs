using AmmoBag.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace AmmoBag
{
    [BepInPlugin(ModInfo.modGUID, ModInfo.modName, ModInfo.modVersion)]
    [BepInDependency("evaisa.lethallib")]
    public class BeltBagPatchBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(ModInfo.modGUID);
        private static BeltBagPatchBase instance;
        internal static GameObject networkAmmo;

        internal static ManualLogSource logSource;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(ModInfo.modGUID);

            networkAmmo = new GameObject("AmmoNetworking");
            networkAmmo.AddComponent<NetworkObject>();
            networkAmmo.AddComponent<AmmoNetworking>();
            networkAmmo.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(networkAmmo);

            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(networkAmmo);

            harmony.PatchAll(typeof(BeltBagPatchBase));
            harmony.PatchAll(typeof(ShotgunPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(NetworkPatch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));

            logSource.LogInfo(ModInfo.modName + " (version - " + ModInfo.modVersion + ")" + ": patches applied successfully");
        }
    }
}
