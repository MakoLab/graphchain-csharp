using System;

namespace elephant.core.exception
{
    public class CreatingBlockException : Exception
    {
        public CreatingBlockException(string message) : base(message)
        {
        }

        public CreatingBlockException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
