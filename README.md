# ASP.NET Core 2.1 API entity data-serialization benchmarks
Run `./run.ps1` or `./run.sh` at the repository root to repeat the experiment

## Question

What is the most performant method of data serialization for resources served by ASP.NET Core 2.1 APIs?

## Variables

Three categories of serialization are tested:

- JSON
- CSV (with a single `StringBuilder` implementation)
- byte[]

Within the JSON category, these methodologies are tested:

- StringBuilder used to append values to string returned by .ToString() override
- The `Jil` JSON serialization library, version `2.15.4` with optional `excludeNulls` behavior
- The `Newtonsoft.Json` JSON serialization library, version `11.0.2`

`Newtonsoft.Json`, `Jil`, CSV, and byte[] scenarios are also tested on an in-memory ASP.NET Core web host

Within the byte[] category, these methodologies are tested:

- `Buffer.BlockCopy`
- `Span<byte>`
- `ReadOnlyMemory<byte>` using `CDorst.Common.Extensions.Memory
- `ReadOnlySpan<byte>` using `CDorst.Common.Extensions.Memory

## Hypothesis

`Jil` is expected to be more performant than `Newtonsoft.Json` based on the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) work

`StringBuilder` is expected to perform well given the benchmarking published in [this blog post](https://blogs.msdn.microsoft.com/dotnet/2018/04/18/performance-improvements-in-net-core-2-1/).

CSV should perform much better than JSON since it is schema-less.

Byte-array block copy should perform even better than CSV since it is also schema-less and contains less data

The low-level ReadOnly Memory/Span types are expected to be more performant than the `Buffer.BlockCopy` implementation due to the simplicity of the serialization operation

## Results

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300-rc1-008673
  [Host]     : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT
  Job-NLOWIL : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT

LaunchCount=5  

```
|                             Method |          Mean |       Error |        StdDev |        Median | Rank |
|----------------------------------- |--------------:|------------:|--------------:|--------------:|-----:|
|               BlockCopyDeserialize |      43.03 ns |   0.3636 ns |     0.9055 ns |      42.99 ns |    7 |
|          BlockCopyDeserializeByRef |      42.41 ns |   0.3587 ns |     0.8999 ns |      42.32 ns |    6 |
|                 BlockCopySerialize |      22.80 ns |   0.1218 ns |     0.3034 ns |      22.79 ns |    3 |
|      ByteSerializeSpanBitConverter |      31.15 ns |   0.4196 ns |     1.1201 ns |      30.78 ns |    5 |
|    ByteSerializeSpanStructReadonly |      44.60 ns |   0.3795 ns |     0.9451 ns |      44.54 ns |    8 |
|        ByteSerializeReadOnlyMemory |      16.73 ns |   0.1036 ns |     0.2581 ns |      16.69 ns |    2 |
|          ByteSerializeReadOnlySpan |      15.95 ns |   0.2536 ns |     0.6500 ns |      15.68 ns |    1 |
|                BlockCopyMixedBytes |      26.92 ns |   0.3848 ns |     1.0534 ns |      26.62 ns |    4 |
|                   StringBuilderCsv |     123.02 ns |   0.8848 ns |     2.2841 ns |     123.14 ns |    9 |
|                   JilJsonSerialize |     413.82 ns |   2.8185 ns |     7.0711 ns |     412.95 ns |   10 |
|                 JilJsonDeserialize |     432.74 ns |   4.2432 ns |    10.6454 ns |     434.98 ns |   11 |
|                  StringBuilderJson |     446.68 ns |   3.5742 ns |     9.6019 ns |     446.43 ns |   12 |
|                     NewtonsoftJson |   1,451.35 ns |  10.8467 ns |    27.8042 ns |   1,451.94 ns |   13 |
|           AspNetCoreApiDefaultJson | 118,376.84 ns | 537.9239 ns | 1,339.6172 ns | 118,268.07 ns |   20 |
|      AspNetCoreApiJilJsonFormatter | 115,077.96 ns | 533.0469 ns | 1,307.5730 ns | 114,899.23 ns |   19 |
|   AspNetCoreApiJilJsonActionResult | 104,425.07 ns | 511.8699 ns | 1,274.7337 ns | 104,504.53 ns |   17 |
|                   AspNetCoreApiCsv |  97,215.99 ns | 548.9245 ns | 1,387.2021 ns |  96,854.21 ns |   16 |
|             AspNetCoreApiByteArray | 110,742.96 ns | 499.3042 ns | 1,252.6565 ns | 110,467.56 ns |   18 |
| AspNetCoreApiByteArrayActionResult |  94,699.81 ns | 383.2812 ns |   947.3762 ns |  94,692.22 ns |   15 |
|     AspNetCoreApiReadOnlyMemoryMvc |  95,158.69 ns | 739.5014 ns | 1,827.8642 ns |  94,810.39 ns |   15 |
|  AspNetCoreApiReadOnlyMemoryRouter |  74,554.79 ns | 376.4867 ns |   937.5826 ns |  74,480.15 ns |   14 |

### Data Size

CSV byte[] length: 15

JSON byte[] length: 72

Bytes byte[] length: 8

CSV string is 4.8x more compact than JSON representation

ToBytes result is 9.0x more compact than JSON representation and 1.9x more compact than CSV

### API Response Time

In-memory ASP.NET Core web server Jil JSON endpoint responds 13.36% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server CSV endpoint responds 21.77% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server byte[] endpoint responds 25.00% faster than default JsonFormatter endpoint

### Proof of Working byte[] Serialization

Entity serialized and deserialized to and from bytes printed as json: `{"EntityId":1000000,"ForeignKeyOneId":1000001,"ForeignKeyTwoId":1000002}`

## Conclusion

byte[] (especially ReadOnlyMemory/Span) serialization outperforms other methods in terms of data-size, serialization runtime, and API request-response runtime.

The resultant Data Table indicates that the in-memory ASP.NET Core server is less performant in handling object results (with or without a Formatter attribute) than when handling IActionResults

## Future Research

Compare the Jil with JilFormatterAttribute, Jil with JsonActionResult, CSV, and byte[] endpoint performance using a non in-memory test infrastructure like the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) project.

Demonstration of better performance with Jil IActionResult scenario than JilFormatterAttribute scenario could yield an improved aspnetcore-mvc-linux Json Serialization [TechempowerBenchmarks](https://github.com/aspnet/benchmarks/blob/d4f95d12d5759feff49a03aa0e432ae7a79ebd6c/src/Benchmarks/Controllers/HomeController.cs#L28-L34) score

