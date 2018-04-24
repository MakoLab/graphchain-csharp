using System;

namespace elephant.core.exception
{
    public class BlockNotFoundException : ReadingBlockException
    {
        public BlockNotFoundException(string message) : base(message)
        {
        }

        public BlockNotFoundException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
