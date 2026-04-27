using GameNetcodeStuff;

namespace AmmoBag
{
    internal static class AmmoBagHelper
    {
        internal const int encode_base_number = 1000000;

        internal static ulong pendingBagNetworkObjectId;
        internal static int pendingAmmoInBagIndex;
        internal static int FindAmmoInBelt(PlayerControllerB player, ShotgunItem __instance)
        {
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                if (CheckSlot(__instance, player.ItemSlots[i]))
                {
                    BeltBagPatchBase.logSource.LogInfo("Found shell in slot " + i);
                    return encode_base_number;
                }
            }

            if (CheckSlot(__instance, player.ItemOnlySlot))
            {
                BeltBagPatchBase.logSource.LogInfo("Found shell in utilitySlot");
                return encode_base_number;
            }

            BeltBagPatchBase.logSource.LogInfo("No shells found");
            return -1;
        }

        private static bool CheckSlot(ShotgunItem __instance, GrabbableObject slot)
        {
            if (slot == null) return false;
            BeltBagItem beltBag = slot as BeltBagItem;
            if (beltBag == null) return false;

            int ammoInBagIndex = FindAmmoInBag(beltBag, __instance.gunCompatibleAmmoID);
            if (ammoInBagIndex == -1) return false;

            pendingBagNetworkObjectId = beltBag.GetComponent<Unity.Netcode.NetworkObject>().NetworkObjectId;
            pendingAmmoInBagIndex = ammoInBagIndex;
            return true;
        }

        private static int FindAmmoInBag(BeltBagItem bag, int ammoTypeID)
        {
            for (int i = bag.objectsInBag.Count - 1; i >= 0; i--)
            {
                var obj = bag.objectsInBag[i];
                if (obj == null) continue;
                var ammo = obj as GunAmmo;
                if (ammo != null && ammo.ammoType == ammoTypeID) return i;
            }
            return -1;
        }
    }
}
