using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace elephant.core.unittests
{
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void ScheduleTasksInMethod()
        {
            Console.WriteLine("Starting task");
            ExecTask();
            Console.WriteLine("Doing something more");
            Thread.Sleep(5000);
        }

        private void ExecTask()
        {
            Console.WriteLine("Executing task");
            var t = RunTask();
        }

        private async Task RunTask()
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
            }
        }
    }
}
