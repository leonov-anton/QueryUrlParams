using BenchmarkDotNet.Running;

namespace QueryUrlParams.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<QueryUrlParamsBenchmarks>();
        }
    }
}
