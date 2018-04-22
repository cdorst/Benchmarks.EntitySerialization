using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;

namespace JsonBenchmarks
{
    [CoreJob]
    [RPlotExporter, RankColumn]
    public class JsonTests
    {
        [Benchmark]
        public byte[] BlockCopyIntegerBytes() => Entity.Default.ToBytes();
        [Benchmark]
        public byte[] BlockCopyMixedBytes() => MixedKeyTypes.Default.ToBytes();
        [Benchmark]
        public string StringBuilderCsv() => Entity.Default.ToStringCsv();
        [Benchmark]
        public string JilJson() => Jil.JSON.Serialize(Entity.Default);
        [Benchmark]
        public string StringBuilderJson() => Entity.Default.ToString();
        [Benchmark]
        public string NewtonsoftJson() => JsonConvert.SerializeObject(Entity.Default);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<JsonTests>();
            var csvString = Entity.Default.ToStringCsv();
            var csvLength = csvString.Length;
            var jsonString = Entity.Default.ToString();
            var jsonLength = jsonString.Length;
            Console.WriteLine($"CSV string: {csvString}");
            Console.WriteLine($"JSON string: {jsonString}");
            Console.WriteLine($"Length of CSV string: {csvLength}");
            Console.WriteLine($"Length of JSON string: {jsonLength}");
            Console.WriteLine($"CSV string contains {((decimal)csvLength / jsonLength).ToString("p")} as much data as the JSON representation");
        }
    }
}
