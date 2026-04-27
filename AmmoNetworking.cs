using LethalNetworkAPI;
using Unity.Netcode;

namespace AmmoBag
{
    internal static class AmmoNetworking
    {
        private static LNetworkMessage<NetworkRequestStruct> msg_deleteShell_Server;
        private static LNetworkMessage<NetworkRequestStruct> msg_deleteShell_Client;
        internal static bool isHosting = false;

        internal static void SetNetworkMessage()
        {
            msg_deleteShell_Server = LNetworkMessage<NetworkRequestStruct>.Connect("msg_DeleteOnServer", onServerReceived: OnRemoveAmmoReceivedServer);
            msg_deleteShell_Client = LNetworkMessage<NetworkRequestStruct>.Connect("msg_DeleteOnClients", onClientReceived: OnRemoveAmmoReceivedClient);
        }

        public static void RequestRemoveAmmo(ulong bagNetworkObjectId, int ammoInBagIndex)
        {
            BeltBagPatchBase.logSource.LogInfo("Requested remove ammo");
            msg_deleteShell_Server.SendServer(new NetworkRequestStruct(bagNetworkObjectId, ammoInBagIndex));
        }

        private static void OnRemoveAmmoReceivedServer(NetworkRequestStruct data, ulong senderClientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(data.bagNetworkObjectId, out var bagNetObj))
            {
                var bag = bagNetObj.GetComponent<BeltBagItem>();
                if (bag != null && data.ammoInBagIndex >= 0 && data.ammoInBagIndex < bag.objectsInBag.Count)
                {
                    bag.objectsInBag.RemoveAt(data.ammoInBagIndex);
                    BeltBagPatchBase.logSource.LogInfo("Host removed shell from bag");
                }
                else
                {
                    BeltBagPatchBase.logSource.LogWarning("SyncBagAmmoRemovalServerRpc: bag null or index out of range");
                }
            }
            else
            {
                BeltBagPatchBase.logSource.LogWarning("SyncBagAmmoRemovalServerRpc: could not find bag with id " + data.bagNetworkObjectId);
            }

            msg_deleteShell_Client.SendClients(new NetworkRequestStruct(data.bagNetworkObjectId, data.ammoInBagIndex));
        }

        private static void OnRemoveAmmoReceivedClient(NetworkRequestStruct @struct)
        {
            if (isHosting) return; //don't trigger again on host

            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(@struct.bagNetworkObjectId, out var bagNetObj))
            {
                BeltBagPatchBase.logSource.LogWarning("SyncBagAmmoRemoval: could not find bag with NetworkObjectId " + @struct.bagNetworkObjectId);
                return;
            }

            var bag = bagNetObj.GetComponent<BeltBagItem>();
            if (bag == null)
            {
                BeltBagPatchBase.logSource.LogWarning("SyncBagAmmoRemoval: NetworkObject is not a BeltBagItem");
                return;
            }

            if (@struct.ammoInBagIndex < 0 || @struct.ammoInBagIndex >= bag.objectsInBag.Count)
            {
                BeltBagPatchBase.logSource.LogWarning("SyncBagAmmoRemoval: ammoInBagIndex out of range");
                return;
            }

            bag.objectsInBag.RemoveAt(@struct.ammoInBagIndex);
        }
    }
}
