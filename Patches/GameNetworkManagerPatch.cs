using HarmonyLib;
using Unity.Netcode;

namespace AmmoBag.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void OnStartPatch()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                var instance = UnityEngine.Object.Instantiate(BeltBagPatchBase.networkAmmo);
                instance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
