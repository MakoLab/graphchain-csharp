using System.Collections.Generic;

namespace elephant.validator.model
{
    public class GraphChainValidationResult
    {
        public bool IsValid { get; private set; }
        public List<BlockValidationResult> Blocks { get; private set; }

        public GraphChainValidationResult(bool valid, List<BlockValidationResult> blocks)
        {
            IsValid = valid;
            Blocks = blocks;
        }

        public void AddBlock(BlockValidationResult blockValidationResult)
        {
            Blocks.Add(blockValidationResult);
        }

        public override string ToString()
        {
            return "GraphChainValidationResult{" +
                "valid=" + IsValid +
                ", blocks=" + Blocks +
                '}';
        }
    }
}
