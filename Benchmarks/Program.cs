using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JsonBenchmarks
{
    public static class Constants
    {
        public static readonly byte[] Bytes = new Entity().ToBytes();
        public static readonly Entity Entity = new Entity();
        public static readonly int EntityId = new Entity().EntityId;
        public static readonly Jil.Options JilOptions = new Jil.Options(excludeNulls: true);
        public static readonly string Json = Jil.JSON.Serialize(Entity, JilOptions);
        public static readonly HttpClient Server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AspNetStartup>().ConfigureLogging(logging => logging.ClearProviders())).CreateClient();
        public static readonly HttpClient ServerUsingRouter = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AspNetStartupUseRouter>().ConfigureLogging(logging => logging.ClearProviders())).CreateClient();
    }

    [SimpleJob(5)]
    [RPlotExporter, RankColumn]
    public class Tests
    {
        [Benchmark]
        public Entity BlockCopyDeserialize() => Entity.FromBytes(Constants.Bytes, Constants.EntityId);
        [Benchmark]
        public Entity BlockCopyDeserializeByRef() => Entity.FromBytesInRef(Constants.Bytes, Constants.EntityId);

        [Benchmark]
        public byte[] BlockCopySerialize() => Constants.Entity.ToBytes();
        [Benchmark]
        public ReadOnlySpan<byte> ByteSerializeSpanBitConverter() => Constants.Entity.ToBytesSpanBitConverter();
        [Benchmark]
        public ReadOnlySpan<byte> ByteSerializeSpanStructReadonly() => Constants.Entity.ToBytesSpanStruct();
        [Benchmark]
        public ReadOnlyMemory<byte> ByteSerializeReadOnlyMemory() => Constants.Entity.ToBytesReadonlyMemory();
        [Benchmark]
        public ReadOnlySpan<byte> ByteSerializeReadOnlySpan() => Constants.Entity.ToBytesReadonlySpan();
        [Benchmark]
        public Entity ByteDeserializeReadOnlySpanBitConverter() => Entity.FromBytesReadOnlySpanBitConverter(Constants.Bytes.AsSpan(), Constants.EntityId);
        [Benchmark]
        public Entity ByteDeserializeReadOnlySpanBitRefStructs() => Entity.FromBytesReadOnlySpanRefStructs(Constants.Bytes.AsSpan(), Constants.EntityId);

        [Benchmark]
        public byte[] BlockCopyMixedBytes() => MixedKeyTypes.Default.ToBytes();
        [Benchmark]
        public string StringBuilderCsv() => Constants.Entity.ToStringCsv();
        [Benchmark]
        public string JilJsonSerialize() => Jil.JSON.Serialize(Constants.Entity, Constants.JilOptions);
        [Benchmark]
        public Entity JilJsonDeserialize() => Jil.JSON.Deserialize<Entity>(Constants.Json);
        [Benchmark]
        public string StringBuilderJson() => Constants.Entity.ToString();
        [Benchmark]
        public string NewtonsoftJson() => JsonConvert.SerializeObject(Constants.Entity);

        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiDefaultJson() => await Constants.Server.GetAsync("/resource/json-default");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiJilJsonFormatter() => await Constants.Server.GetAsync("/resource/json-formatter");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiJilJsonActionResult() => await Constants.Server.GetAsync("/resource/json-actionresult");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiCsv() => await Constants.Server.GetAsync("/resource/csv");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiByteArray() => await Constants.Server.GetAsync("/resource/bytes");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiByteArrayActionResult() => await Constants.Server.GetAsync("/resource/bytes-actionresult");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiReadOnlyMemoryMvc() => await Constants.Server.GetAsync("/resource/bytes-readonlymemory");
        [Benchmark]
        public async Task<HttpResponseMessage> AspNetCoreApiReadOnlyMemoryRouter() => await Constants.ServerUsingRouter.GetAsync("/resource");
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Tests>();
            var dataTable = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", "results", "Tests-report-github.md"));
            var resultSummary = GetResultSummary(dataTable).Split(Environment.NewLine);
            foreach (var line in resultSummary) Console.WriteLine(line);
            var readme = new StringBuilder()
                .Append("# ASP.NET Core 2.1 API entity data-serialization benchmarks")
                .AppendLine()
                .AppendLine("Run `./run.ps1` or `./run.sh` at the repository root to repeat the experiment")
                .AppendLine()
                .AppendLine("## Question")
                .AppendLine()
                .AppendLine("What is the most performant method of data serialization for resources served by ASP.NET Core 2.1 APIs?")
                .AppendLine()
                .AppendLine("## Variables")
                .AppendLine()
                .AppendLine("Three categories of serialization are tested:")
                .AppendLine()
                .AppendLine("- JSON")
                .AppendLine("- CSV (with a single `StringBuilder` implementation)")
                .AppendLine("- byte[]")
                .AppendLine()
                .AppendLine("Within the JSON category, these methodologies are tested:")
                .AppendLine()
                .AppendLine("- StringBuilder used to append values to string returned by .ToString() override")
                .AppendLine("- The `Jil` JSON serialization library, version `2.15.4` with optional `excludeNulls` behavior")
                .AppendLine("- The `Newtonsoft.Json` JSON serialization library, version `11.0.2`")
                .AppendLine()
                .AppendLine("`Newtonsoft.Json`, `Jil`, CSV, and byte[] scenarios are also tested on an in-memory ASP.NET Core web host")
                .AppendLine()
                .AppendLine("Within the byte[] category, these methodologies are tested:")
                .AppendLine()
                .AppendLine("- `Buffer.BlockCopy`")
                .AppendLine("- `Span<byte>`")
                .AppendLine("- `ReadOnlyMemory<byte>` using `CDorst.Common.Extensions.Memory")
                .AppendLine("- `ReadOnlySpan<byte>` using `CDorst.Common.Extensions.Memory")
                .AppendLine()
                .AppendLine("## Hypothesis")
                .AppendLine()
                .AppendLine("`Jil` is expected to be more performant than `Newtonsoft.Json` based on the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) work")
                .AppendLine()
                .AppendLine("`StringBuilder` is expected to perform well given the benchmarking published in [this blog post](https://blogs.msdn.microsoft.com/dotnet/2018/04/18/performance-improvements-in-net-core-2-1/).")
                .AppendLine()
                .AppendLine("CSV should perform much better than JSON since it is schema-less.")
                .AppendLine()
                .AppendLine("Byte-array block copy should perform even better than CSV since it is also schema-less and contains less data")
                .AppendLine()
                .AppendLine("The low-level ReadOnly Memory/Span types are expected to be more performant than the `Buffer.BlockCopy` implementation due to the simplicity of the serialization operation")
                .AppendLine()
                .AppendLine("## Results")
                .AppendLine();
            foreach (var line in dataTable) readme.AppendLine(line);
            readme.AppendLine();
            foreach (var line in resultSummary) readme.AppendLine(line);
            readme
                .AppendLine("## Conclusion")
                .AppendLine()
                .AppendLine("byte[] (especially ReadOnlyMemory/Span) serialization outperforms other methods in terms of data-size, serialization runtime, and API request-response runtime.")
                .AppendLine()
                .AppendLine("The resultant Data Table indicates that the in-memory ASP.NET Core server is less performant in handling object results (with or without a Formatter attribute) than when handling IActionResults")
                .AppendLine()
                .AppendLine("## Future Research")
                .AppendLine()
                .AppendLine("Compare the Jil with JilFormatterAttribute, Jil with JsonActionResult, CSV, and byte[] endpoint performance using a non in-memory test infrastructure like the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) project.")
                .AppendLine()
                .AppendLine("Demonstration of better performance with Jil IActionResult scenario than JilFormatterAttribute scenario could yield an improved aspnetcore-mvc-linux Json Serialization [TechempowerBenchmarks](https://github.com/aspnet/benchmarks/blob/d4f95d12d5759feff49a03aa0e432ae7a79ebd6c/src/Benchmarks/Controllers/HomeController.cs#L28-L34) score")
                .AppendLine();
            File.WriteAllText("../README.md", readme.ToString());
        }

        private static string GetResultSummary(string[] dataTable)
        {
            var (apiJsonNetResponseTime, apiJilJsonResponseTime, apiCsvResponseTime, apiBytesResponseTime) = parseApiResponseTimes(dataTable);
            var csvLength = Encoding.UTF8.GetBytes(Constants.Entity.ToStringCsv()).Length;
            var jsonLength = Encoding.UTF8.GetBytes(Constants.Json).Length;
            var bytesLength = Constants.Entity.ToBytes().Length;
            return new StringBuilder()
                .AppendLine("### Data Size")
                .AppendLine()
                .AppendLine($"CSV byte[] length: {csvLength}") // 15
                .AppendLine()
                .AppendLine($"JSON byte[] length: {jsonLength}") // 72
                .AppendLine()
                .AppendLine($"Bytes byte[] length: {bytesLength}") // 8
                .AppendLine()
                .AppendLine($"CSV string is {((decimal)jsonLength / csvLength).ToString("N1")}x more compact than JSON representation")
                .AppendLine()
                .AppendLine($"ToBytes result is {((decimal)jsonLength / bytesLength).ToString("N1")}x more compact than JSON representation and {((decimal)csvLength / bytesLength).ToString("N1")}x more compact than CSV")
                .AppendLine()
                .AppendLine("### API Response Time")
                .AppendLine()
                .AppendLine(CompareResponseTime(apiJsonNetResponseTime, apiJilJsonResponseTime, "Jil JSON"))
                .AppendLine()
                .AppendLine(CompareResponseTime(apiJsonNetResponseTime, apiCsvResponseTime, "CSV"))
                .AppendLine()
                .AppendLine(CompareResponseTime(apiJsonNetResponseTime, apiBytesResponseTime, "byte[]"))
                .AppendLine()
                .AppendLine("### Proof of Working byte[] Serialization")
                .AppendLine()
                .AppendLine($"Entity serialized and deserialized to and from bytes printed as json: `{Jil.JSON.Serialize(Entity.FromBytes(Constants.Bytes, Constants.Entity.EntityId), Constants.JilOptions)}`")
                .ToString();
        }

        private static string CompareResponseTime(decimal slowResponseTime, decimal fastResponseTime, string label)
            => $"In-memory ASP.NET Core web server {label} endpoint responds {(slowResponseTime / fastResponseTime - 1).ToString("p")} faster than default JsonFormatter endpoint";

        private static (decimal apiJsonNetResponseTime, decimal apiJilJsonResponseTime, decimal apiCsvResponseTime, decimal apiBytesResponseTime) parseApiResponseTimes(string[] dataTable)
            => (
            parseResponseTime(dataTable, nameof(Tests.AspNetCoreApiDefaultJson)),
            parseResponseTime(dataTable, nameof(Tests.AspNetCoreApiJilJsonActionResult)),
            parseResponseTime(dataTable, nameof(Tests.AspNetCoreApiCsv)),
            parseResponseTime(dataTable, nameof(Tests.AspNetCoreApiByteArrayActionResult)));

        private static decimal parseResponseTime(string[] dataTable, string method)
            => decimal.Parse(dataTable.First(line => line.Contains(method)).Split('|').Skip(2).First().Replace(",", "").Replace("ns", "").Trim());
    }
}
