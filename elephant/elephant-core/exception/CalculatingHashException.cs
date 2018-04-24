using System;

namespace elephant.core.exception
{
    public class CalculatingHashException : Exception
    {
        public CalculatingHashException(String message) : base(message)
        {
        }

        public CalculatingHashException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}
