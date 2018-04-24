using System;

namespace elephant.core.exception
{
    public class ObtainingBlockHeaderException : Exception
    {
        public ObtainingBlockHeaderException(string message) : base(message)
        {
        }

        public ObtainingBlockHeaderException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
