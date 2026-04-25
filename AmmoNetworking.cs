using Unity.Netcode;

namespace AmmoBag.Patches
{
    internal class AmmoNetworking : NetworkBehaviour
    {
        public static AmmoNetworking Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            Instance = this;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DestroyShellServerRpc(NetworkObjectReference netObjRef)
        {
            if (netObjRef.TryGet(out var netObj))
            {
                BeltBagPatchBase.logSource.LogInfo("Despawn shell");
                netObj.Despawn(destroy: true);
            }
        }
    }
}
