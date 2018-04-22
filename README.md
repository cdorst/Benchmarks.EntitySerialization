# ASP.NET Core 2.1 data serialization benchmarks for a typical entity
Run `./run.ps1` at the repository root to repeat the experiment

## Question

What is the most performant method of data serialization for resources served by ASP.NET Core 2.1 APIs?

## Variables

Three categories of serialization are tested:

- JSON
- CSV (with a single `StringBuilder` implementation)
- byte[] (with a single `Buffer.BlockCopy()` implementation)

Within the JSON category, three different methodologies of serializing JSON are tested:

- StringBuilder used to append values to string returned by .ToString() override
- The `Jil` JSON serialization library, version `2.15.4` with optional `excludeNulls` behavior
- The `Newtonsoft.Json` JSON serialization library, version `11.0.2`

`Newtonsoft.Json`, `Jil`, CSV, and byte[] scenarios are also tested on an in-memory ASP.NET Core web host

## Hypothesis

`Jil` is expected to be more performant than `Newtonsoft.Json` based on the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) work

`StringBuilder` is expected to perform well given the benchmarking published in [this blog post](https://blogs.msdn.microsoft.com/dotnet/2018/04/18/performance-improvements-in-net-core-2-1/).

CSV should perform much better than JSON since it is schema-less.

Byte-array block copy should perform even better than CSV since it is also schema-less and contains less data

## Results

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.371 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
Frequency=2742186 Hz, Resolution=364.6726 ns, Timer=TSC
.NET Core SDK=2.1.300-preview2-008533
  [Host] : .NET Core 2.1.0-preview2-26406-04 (CoreCLR 4.6.26406.07, CoreFX 4.6.26406.04), 64bit RyuJIT
  Core   : .NET Core 2.1.0-preview2-26406-04 (CoreCLR 4.6.26406.07, CoreFX 4.6.26406.04), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                             Method |          Mean |         Error |        StdDev | Rank |
|----------------------------------- |--------------:|--------------:|--------------:|-----:|
|               BlockCopyDeserialize |      40.74 ns |     0.3683 ns |     0.3265 ns |    3 |
|                 BlockCopySerialize |      21.66 ns |     0.3769 ns |     0.3341 ns |    1 |
|                BlockCopyMixedBytes |      25.73 ns |     0.6056 ns |     0.7659 ns |    2 |
|                   StringBuilderCsv |     153.14 ns |     1.6019 ns |     1.3376 ns |    4 |
|                   JilJsonSerialize |     395.18 ns |     3.3381 ns |     3.1225 ns |    5 |
|                 JilJsonDeserialize |     415.73 ns |     1.1122 ns |     0.8042 ns |    6 |
|                  StringBuilderJson |     500.10 ns |     5.8911 ns |     5.5106 ns |    7 |
|                     NewtonsoftJson |   1,393.67 ns |    17.0376 ns |    15.9370 ns |    8 |
|           AspNetCoreApiDefaultJson | 103,404.97 ns | 1,351.0708 ns | 1,128.2052 ns |   14 |
|      AspNetCoreApiJilJsonFormatter | 101,058.29 ns | 1,167.5564 ns | 1,092.1328 ns |   13 |
|   AspNetCoreApiJilJsonActionResult |  90,261.51 ns |   855.3045 ns |   714.2180 ns |   11 |
|                   AspNetCoreApiCsv |  81,342.03 ns | 1,526.0148 ns | 1,567.1047 ns |   10 |
|             AspNetCoreApiByteArray |  97,260.16 ns | 1,025.2694 ns |   908.8745 ns |   12 |
| AspNetCoreApiByteArrayActionResult |  78,722.34 ns | 1,376.4602 ns | 1,149.4065 ns |    9 |

### Data Size

CSV byte[] length: 15

JSON byte[] length: 72

Bytes byte[] length: 8

CSV string is 4.8x more compact than JSON representation

ToBytes result is 9.0x more compact than JSON representation and 1.9x more compact than CSV

Deserialized bytes entity as json: `{"EntityId":1000000,"ForeignKeyOneId":1000001,"ForeignKeyTwoId":1000002}`

### API Response Time

In-memory ASP.NET Core web server Jil JSON endpoint responds 14.56% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server CSV endpoint responds 27.12% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server byte[] endpoint responds 31.35% faster than default JsonFormatter endpoint

## Conclusion

byte[] block-copy serialization outperformed other methods in terms of data-size, serialization runtime, and API request-response runtime.

The resultant Data Table indicates that the in-memory ASP.NET Core server is less performant in handling object results (with or without a Formatter attribute) than when handling IActionResults

## Future Research

Compare the Jil with JilFormatterAttribute, Jil with JsonActionResult, CSV, and byte[] endpoint performance using a non in-memory test infrastructure like the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) project.

Discovery of better performance with Jil IActionResult scenario than JilFormatterAttribute scenario could yield an improved MvcJson score

