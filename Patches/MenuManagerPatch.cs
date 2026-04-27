using HarmonyLib;

namespace AmmoBag.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch
    {
        [HarmonyPatch("StartHosting")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void OnStartHostingPatch()
        {
            AmmoNetworking.isHosting = true;
        }

        [HarmonyPatch("StartAClient")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void OnStartConnectingPatch()
        {
            AmmoNetworking.isHosting = false;
        }
    }
}
