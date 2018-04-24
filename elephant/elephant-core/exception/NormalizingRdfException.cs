using System;

namespace elephant.core.exception
{
    public class NormalizingRdfException : Exception
    {
        public NormalizingRdfException(string message) : base(message)
        {
        }

        public NormalizingRdfException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
