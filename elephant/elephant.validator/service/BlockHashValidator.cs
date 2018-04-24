using System;
using System.Globalization;
using System.Text;
using elephant.core.model.chain;
using elephant.core.service.cryptography;
using elephant.validator.model;

namespace elephant.validator.service
{
    public class BlockHashValidator
    {
        //private ILogger<BlockHashValidator> _logger;
        private Sha256HashCalculator _hashCalculator;

        public BlockHashValidator()
        {
            _hashCalculator = new Sha256HashCalculator();
        }

        public BlockValidationResult ValidateBlock(Block blockToValidate, string calculatedDataHash)
        {
            BlockHeader blockHeader = blockToValidate.BlockHeader;

            StringBuilder sb = new StringBuilder();
            sb.Append(FromDecimalToLong(blockHeader.Index));
            sb.Append(blockHeader.PreviousBlock);
            sb.Append(blockHeader.PreviousHash);
            sb.Append(FromDecimalToLong(blockHeader.Timestamp));
            sb.Append(blockHeader.DataGraphIri);
            sb.Append(calculatedDataHash.ToLower());

            string currentBlockHash = blockHeader.Hash.ToLower();
            string calculatedBlockHash = _hashCalculator.CalculateHash(sb.ToString()).ToLower();

            if (calculatedBlockHash.Equals(currentBlockHash))
            {
                return new BlockValidationResult(true, calculatedBlockHash, calculatedDataHash, blockHeader);
            }
            else
            {
                return new BlockValidationResult(false, calculatedBlockHash, calculatedDataHash, blockHeader);
            }
        }

        private string FromDecimalToLong(string number)
        {
            return Convert.ToInt64(Double.Parse(number, CultureInfo.InvariantCulture)).ToString();
        }
    }
}
