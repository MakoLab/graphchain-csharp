namespace elephant.core.model.chain
{
    public class LastBlockInfo
    {
        public string BlockIri { get; private set; }
        public string BlockHash { get; private set; }
        public string BlockIndex { get; private set; }

        public LastBlockInfo(string blockIri, string blockHash, string blockIndex)
        {
            BlockIri = blockIri;
            BlockHash = blockHash;
            BlockIndex = blockIndex;
        }

        public override string ToString()
        {
            return "LastBlockInfo{" +
                "blockIri='" + BlockIri + '\'' +
                ", blockHash='" + BlockHash + '\'' +
                ", blockIndex='" + BlockIndex + '\'' +
                '}';
        }
    }
}
