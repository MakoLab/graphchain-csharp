namespace elephant.core.model.chain
{
    public class Block
    {
        public string BlockIri { get; private set; }
        public BlockHeader BlockHeader { get; private set; }
        public BlockContent BlockContent { get; private set; }
        public bool IsGenesisBlock { get; private set; }

        public Block(string blockIri, BlockHeader blockHeader, BlockContent blockContent)
            : this(blockIri, blockHeader, blockContent, false)
        {
        }

        public Block(string blockIri, BlockHeader blockHeader, BlockContent blockContent, bool genesisBlock)
        {
            BlockIri = blockIri;
            BlockHeader = blockHeader;
            BlockContent = blockContent;
            IsGenesisBlock = genesisBlock;
        }
    }
}
