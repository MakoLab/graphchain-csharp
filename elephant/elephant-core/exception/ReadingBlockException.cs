using System;

namespace elephant.core.exception
{
    public class ReadingBlockException : Exception
    {
        public ReadingBlockException(string message) : base(message)
        {
        }

        public ReadingBlockException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
