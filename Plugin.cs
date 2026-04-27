using AmmoBag.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AmmoBag
{
    [BepInPlugin(ModInfo.modGUID, ModInfo.modName, ModInfo.modVersion)]
    [BepInDependency("LethalNetworkAPI")]
    public class BeltBagPatchBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(ModInfo.modGUID);
        private static BeltBagPatchBase instance;

        internal static ManualLogSource logSource;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(ModInfo.modGUID);

            harmony.PatchAll(typeof(BeltBagPatchBase));
            harmony.PatchAll(typeof(NetworkPatch));
            harmony.PatchAll(typeof(ShotgunPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));

            AmmoNetworking.SetNetworkMessage();

            logSource.LogInfo(ModInfo.modName + " (version - " + ModInfo.modVersion + ")" + ": patches applied successfully");
        }
    }
}
