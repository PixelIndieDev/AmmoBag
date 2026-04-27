using GameNetcodeStuff;
using HarmonyLib;

namespace AmmoBag.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunPatch
    {
        [HarmonyPatch("FindAmmoInInventory")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void PatchFindAmmo(ref int __result, ShotgunItem __instance)
        {
            // no ammo found
            if (__result == -1)
            {
                PlayerControllerB player = __instance.playerHeldBy;
                if (player == null) return;
                __result = AmmoBagHelper.FindAmmoInBelt(player, __instance);
            }
        }
    }
}
