using System;

namespace elephant.core.exception
{
    public class RdfSerializationException : Exception
    {
        public RdfSerializationException(string message) : base(message)
        {
        }

        public RdfSerializationException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
