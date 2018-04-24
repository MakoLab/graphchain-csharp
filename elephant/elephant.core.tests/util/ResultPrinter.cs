using System;
using System.Collections.Generic;
using elephant.core.tests.model;

namespace elephant.core.tests.util
{
    public class ResultPrinter
    {
        public static void PrintResults(string testSuiteName, List<BenchmarkResult> results)
        {
            Console.WriteLine(testSuiteName + " has finished. Tests results:");

            foreach (BenchmarkResult result in results)
            {
                string msg = String.Format("{0} -> {1}", result.Size, result.TimeInMillis);
                Console.WriteLine(msg);
            }
        }
    }
}
