using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace AmmoBag.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerPatch
    {
        [HarmonyPatch("DestroyItemInSlotAndSync")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static bool PatchFindAmmo(PlayerControllerB __instance, int itemSlot)
        {
            if (itemSlot < AmmoBagHelper.encode_base_number) return true;

            ulong bagNetworkObjectId = AmmoBagHelper.pendingBagNetworkObjectId;
            int ammoInBagIndex = AmmoBagHelper.pendingAmmoInBagIndex;

            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bagNetworkObjectId, out var bagNetObj))
            {
                BeltBagPatchBase.logSource.LogWarning("Could not find belt bag NetworkObject with id " + bagNetworkObjectId);
                return false;
            }

            AmmoNetworking.RequestRemoveAmmo(bagNetworkObjectId, ammoInBagIndex);
            return false;
        }
    }
}
