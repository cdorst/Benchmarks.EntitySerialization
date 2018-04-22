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
|               BlockCopyDeserialize |      41.99 ns |     0.5396 ns |     0.4784 ns |    3 |
|                 BlockCopySerialize |      21.82 ns |     0.3174 ns |     0.2814 ns |    1 |
|                BlockCopyMixedBytes |      25.25 ns |     0.3049 ns |     0.2852 ns |    2 |
|                   StringBuilderCsv |     156.72 ns |     2.2644 ns |     2.1182 ns |    4 |
|                   JilJsonSerialize |     400.01 ns |     7.7875 ns |     6.9034 ns |    5 |
|                 JilJsonDeserialize |     434.15 ns |     4.3004 ns |     3.8122 ns |    6 |
|                  StringBuilderJson |     507.62 ns |     4.7838 ns |     4.4747 ns |    7 |
|                     NewtonsoftJson |   1,383.46 ns |    14.2548 ns |    12.6365 ns |    8 |
|           AspNetCoreApiDefaultJson | 106,007.48 ns | 2,102.7376 ns | 2,159.3565 ns |   13 |
|      AspNetCoreApiJilJsonFormatter | 101,393.56 ns | 1,530.0337 ns | 1,356.3349 ns |   12 |
|   AspNetCoreApiJilJsonActionResult |  92,318.09 ns | 1,008.6266 ns |   894.1212 ns |   10 |
|                   AspNetCoreApiCsv |  81,192.38 ns | 1,055.3480 ns |   881.2633 ns |    9 |
|             AspNetCoreApiByteArray |  98,537.08 ns | 1,855.8795 ns | 1,822.7216 ns |   11 |
| AspNetCoreApiByteArrayActionResult |  81,607.05 ns | 1,583.0167 ns | 1,884.4688 ns |    9 |

CSV byte[] length: 15
JSON byte[] length: 72
Bytes byte[] length: 8
CSV string is 4.8x more compact than JSON representation
ToBytes result is 9.0x more compact than JSON representation and 1.9x more compact than CSV

Deserialized bytes entity as json: `{"EntityId":1000000,"ForeignKeyOneId":1000001,"ForeignKeyTwoId":1000002}`

In-memory ASP.NET Core web server Jil JSON endpoint responds 14.83% faster than default JsonFormatter endpoint
In-memory ASP.NET Core web server CSV endpoint responds 30.56% faster than default JsonFormatter endpoint
In-memory ASP.NET Core web server byte[] endpoint responds 29.90% faster than default JsonFormatter endpoint

## Conclusion

byte[] block-copy serialization outperformed other methods in terms of data-size, serialization runtime, and API request-response runtime.

The resultant Data Table indicates that the in-memory ASP.NET Core server is less performant in handling object results (with or without a Formatter attribute) than when handling IActionResults

## Future Research

Compare the Jil with JilFormatterAttribute, Jil with JsonActionResult, CSV, and byte[] endpoint performance using a non in-memory test infrastructure like the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) project.

Discovery of better performance with Jil IActionResult scenario than JilFormatterAttribute scenario could yield an improved MvcJson score

