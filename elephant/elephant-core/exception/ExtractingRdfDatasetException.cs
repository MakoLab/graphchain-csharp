using System;

namespace elephant.core.exception
{
    public class ExtractingRdfDatasetException : Exception
    {
        public ExtractingRdfDatasetException(string message) : base(message)
        {
        }

        public ExtractingRdfDatasetException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
