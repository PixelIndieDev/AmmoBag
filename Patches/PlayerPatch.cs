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
            //if true, no encoded slot. Run vanilla code
            if (itemSlot < AmmoBagHelper.encode_base_number) return true;
            //decode slot
            var (beltBagSlotIndex, isBagInUtilitySlot, ammoInBagIndex) = AmmoBagHelper.DecodeIndex(itemSlot);

            BeltBagItem bag;
            if (isBagInUtilitySlot)
            {
                bag = __instance.ItemOnlySlot as BeltBagItem;
            }
            else
            {
                if (beltBagSlotIndex < 0 || beltBagSlotIndex >= __instance.ItemSlots.Length) return true;
                bag = __instance.ItemSlots[beltBagSlotIndex] as BeltBagItem;
            }

            if (bag == null) return true;

            if (ammoInBagIndex < 0 || ammoInBagIndex >= bag.objectsInBag.Count) return true;

            var shellToRemove = bag.objectsInBag[ammoInBagIndex];
            if (shellToRemove == null) return true;

            bag.objectsInBag.RemoveAt(ammoInBagIndex);

            var netObj = shellToRemove.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                {
                    BeltBagPatchBase.logSource.LogInfo("Player " + __instance.playerClientId + " who called shell delete is SERVER");
                    netObj.Despawn(destroy: true);
                }
                else
                {
                    BeltBagPatchBase.logSource.LogInfo("Player " + __instance.playerClientId + " who called shell delete is CLIENT");
                    AmmoNetworking.Instance.DestroyShellServerRpc(netObj);
                }
            }
            return false;
        }
    }
}
