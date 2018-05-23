using BenchmarkDotNet.Running;
using static Benchmarks.ReadmeFileWriter;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Tests>();
            WriteReadmeFile();
        }
    }
}
