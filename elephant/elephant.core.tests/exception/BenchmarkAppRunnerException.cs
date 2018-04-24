using System;

namespace elephant.core.tests.exception
{
    public class BenchmarkAppRunnerException : Exception
    {
        public BenchmarkAppRunnerException(string message) : base(message)
        {
        }

        public BenchmarkAppRunnerException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
