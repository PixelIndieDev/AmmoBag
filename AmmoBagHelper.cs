using GameNetcodeStuff;

namespace AmmoBag
{
    internal static class AmmoBagHelper
    {
        internal const int encode_base_number = 10000;
        internal static int FindAmmoInBelt(PlayerControllerB player, ShotgunItem __instance)
        {
            int checkedIndex = -1;
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                checkedIndex = CheckSlot(__instance, player.ItemSlots[i], i, false);
                if (checkedIndex == -1) continue;
                //found ammo
                BeltBagPatchBase.logSource.LogInfo("Found shell at " + checkedIndex);
                return checkedIndex;
            }
            //still not found ammo
            checkedIndex = CheckSlot(__instance, player.ItemOnlySlot, -1, true);
            if (checkedIndex != -1) BeltBagPatchBase.logSource.LogInfo("Found shell in utilitySlot at " + checkedIndex);
            else BeltBagPatchBase.logSource.LogInfo("No shells found");
            return checkedIndex;
        }

        private static int CheckSlot(ShotgunItem __instance, GrabbableObject slot, int bagSlotIndex, bool isUtilitySlot)
        {
            if (slot == null) return -1;
            BeltBagItem beltBag = slot as BeltBagItem;
            if (beltBag == null) return -1;

            int ammoInBagIndex = FindAmmoInBag(beltBag, __instance.gunCompatibleAmmoID);
            if (ammoInBagIndex != -1) return EncodeIndex(bagSlotIndex, isUtilitySlot, ammoInBagIndex);
            return -1;
        }

        private static int FindAmmoInBag(BeltBagItem bag, int ammoTypeID)
        {
            for (int i = 0; i < bag.objectsInBag.Count; i++)
            {
                var obj = bag.objectsInBag[i];
                if (obj == null) continue;

                var ammo = obj as GunAmmo;
                if (ammo != null && ammo.ammoType == ammoTypeID) return i;
            }
            return -1;
        }

        private static int EncodeIndex(int beltBagIndex, bool isBagInUtilitySlot, int ammoInBagIndex)
        {
            int slotComponent = isBagInUtilitySlot ? 1000 : (beltBagIndex + 1) * 100;
            return encode_base_number + slotComponent + ammoInBagIndex;
        }

        internal static (int beltBagSlotIndex, bool isBagInUtilitySlot, int ammoInBagIndex) DecodeIndex(int encodedIndex)
        {
            int value = encodedIndex - encode_base_number;
            bool isBagInUtilitySlot = value >= 1000;
            if (isBagInUtilitySlot)
            {
                int ammoInBagIndex = value - 1000;
                return (-1, true, ammoInBagIndex);
            }
            else
            {
                int beltBagSlotIndex = (value / 100) - 1;
                int ammoInBagIndex = value % 100;
                return (beltBagSlotIndex, false, ammoInBagIndex);
            }
        }
    }
}
