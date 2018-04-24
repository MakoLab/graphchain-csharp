using System.Text;
using elephant.core.model.chain;

namespace elephant.validator.model
{
    public class BlockValidationResult
    {
        public bool IsValid { get; private set; }
        public string CalculatedBlockHash { get; private set; }
        public string CalculatedDataHash { get; private set; }
        public string CurrentBlockHash { get; private set; }
        public string CurrentDataHash { get; private set; }


        public BlockValidationResult(bool valid, string calculatedBlockHash, string calculatedDataHash, BlockHeader blockHeader)
        {
            IsValid = valid;
            CalculatedBlockHash = calculatedBlockHash;
            CalculatedDataHash = calculatedDataHash;
            CurrentBlockHash = blockHeader.Hash;
            CurrentDataHash = blockHeader.DataHash;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("BlockValidationResult{");
            sb.Append("valid = ").Append(IsValid).Append(", \n");
            sb.Append("calculatedBlockHash = ").Append(CalculatedBlockHash).Append(", \n");
            sb.Append("calculatedDataHash = ").Append(CalculatedDataHash).Append(", \n");
            sb.Append("currentBlockHash = ").Append(CurrentBlockHash).Append(", \n");
            sb.Append("currentDataHash = ").Append(CurrentDataHash);
            sb.Append("}");

            return sb.ToString();
        }
    }
}
