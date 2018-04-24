using System.Text;

namespace elephant.core.tests.model
{
    public class BenchmarkResult
    {
        public long Size { get; private set; }
        public long TimeInMillis { get; private set; }

        public BenchmarkResult(long size, long timeInMillis)
        {
            Size = size;
            TimeInMillis = timeInMillis;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("BenchmarkResult{size=");
            sb.Append(Size);
            sb.Append(", timeInMillis=");
            sb.Append(TimeInMillis);
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
