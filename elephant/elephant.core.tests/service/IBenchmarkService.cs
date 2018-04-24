using elephant.core.tests.model;

namespace elephant.core.tests.service
{
    public interface IBenchmarkService
    {
        BenchmarkResult Run(int size);
        string Name { get; }
    }
}
