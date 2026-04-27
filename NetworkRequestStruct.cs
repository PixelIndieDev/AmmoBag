namespace AmmoBag
{
    public struct NetworkRequestStruct
    {
        public ulong bagNetworkObjectId;
        public int ammoInBagIndex;
        public NetworkRequestStruct(ulong bagNetworkObjectId, int ammoInBagIndex)
        {
            this.bagNetworkObjectId = bagNetworkObjectId;
            this.ammoInBagIndex = ammoInBagIndex;
        }
    }
}
