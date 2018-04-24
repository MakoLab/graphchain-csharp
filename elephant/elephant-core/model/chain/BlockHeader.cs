using System;

namespace elephant.core.model.chain
{
    public class BlockHeader
    {
        public string DataGraphIri { get; private set; }
        public string DataHash { get; private set; }
        public string Hash { get; private set; }
        public string Index { get; private set; }
        public string PreviousBlock { get; private set; }
        public string PreviousHash { get; private set; }
        public string Timestamp { get; private set; }

        public BlockHeader(string dataGraphIri, string dataHash, string hash, string index, string previousBlock, string previousHash, string timestamp)
        {
            DataGraphIri = dataGraphIri;
            DataHash = dataHash;
            Hash = hash;
            Index = index;
            PreviousBlock = previousBlock;
            PreviousHash = previousHash;
            Timestamp = timestamp;
        }

        public int GetIndexAsInt()
        {
            return Int32.Parse(Index);
        }

        public override string ToString()
        {
            return "BlockHeader{" +
                "dataGraphIri='" + DataGraphIri + '\'' +
                ", dataHash='" + DataHash + '\'' +
                ", hash='" + Hash + '\'' +
                ", index='" + Index + '\'' +
                ", previousBlock='" + PreviousBlock + '\'' +
                ", previousHash='" + PreviousHash + '\'' +
                ", timestamp='" + Timestamp + '\'' +
                '}';
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            BlockHeader that = (BlockHeader)o;
            return Equals(DataGraphIri, that.DataGraphIri) &&
                Equals(DataHash, that.DataHash) &&
                Equals(Hash, that.Hash) &&
                Equals(Index, that.Index) &&
                Equals(PreviousBlock, that.PreviousBlock) &&
                Equals(PreviousHash, that.PreviousHash) &&
                Equals(Timestamp, that.Timestamp);
        }

        public override int GetHashCode()
        {
            return DataGraphIri.GetHashCode() + DataHash.GetHashCode() + Hash.GetHashCode() + Index.GetHashCode() + PreviousBlock.GetHashCode() + PreviousHash.GetHashCode() + Timestamp.GetHashCode();
        }
    }
}
