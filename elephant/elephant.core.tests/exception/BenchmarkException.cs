using System;

namespace elephant.core.tests.exception
{
    public class BenchmarkException : Exception
    {
        public BenchmarkException(string message) : base(message)
        {
        }

        public BenchmarkException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
