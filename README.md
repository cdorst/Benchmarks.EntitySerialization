# ASP.NET Core 2.1 API entity data-serialization benchmarks
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
|               BlockCopyDeserialize |      40.87 ns |     0.2864 ns |     0.2392 ns |    3 |
|                 BlockCopySerialize |      22.65 ns |     0.5252 ns |     0.5393 ns |    1 |
|                BlockCopyMixedBytes |      25.22 ns |     0.4293 ns |     0.4015 ns |    2 |
|                   StringBuilderCsv |     154.14 ns |     1.0769 ns |     0.9547 ns |    4 |
|                   JilJsonSerialize |     390.83 ns |     4.7882 ns |     4.4789 ns |    5 |
|                 JilJsonDeserialize |     436.18 ns |     3.5067 ns |     3.2801 ns |    6 |
|                  StringBuilderJson |     514.66 ns |     8.9724 ns |     8.3928 ns |    7 |
|                     NewtonsoftJson |   1,359.78 ns |    15.0929 ns |    14.1179 ns |    8 |
|           AspNetCoreApiDefaultJson | 103,762.69 ns | 1,426.3361 ns | 1,334.1956 ns |   14 |
|      AspNetCoreApiJilJsonFormatter |  99,669.44 ns |   987.8077 ns |   923.9959 ns |   13 |
|   AspNetCoreApiJilJsonActionResult |  93,266.24 ns | 1,821.4925 ns | 1,703.8251 ns |   11 |
|                   AspNetCoreApiCsv |  82,944.09 ns | 1,228.5272 ns | 1,025.8758 ns |   10 |
|             AspNetCoreApiByteArray |  95,297.31 ns | 1,105.9473 ns | 1,034.5037 ns |   12 |
| AspNetCoreApiByteArrayActionResult |  78,409.88 ns | 1,591.3476 ns | 2,124.4034 ns |    9 |

### Data Size

CSV byte[] length: 15

JSON byte[] length: 72

Bytes byte[] length: 8

CSV string is 4.8x more compact than JSON representation

ToBytes result is 9.0x more compact than JSON representation and 1.9x more compact than CSV

### API Response Time

In-memory ASP.NET Core web server Jil JSON endpoint responds 11.25% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server CSV endpoint responds 25.10% faster than default JsonFormatter endpoint

In-memory ASP.NET Core web server byte[] endpoint responds 32.33% faster than default JsonFormatter endpoint

### Proof of Working byte[] Serialization

Entity serialized and deserialized to and from bytes printed as json: `{"EntityId":1000000,"ForeignKeyOneId":1000001,"ForeignKeyTwoId":1000002}`

## Conclusion

byte[] block-copy serialization outperformed other methods in terms of data-size, serialization runtime, and API request-response runtime.

The resultant Data Table indicates that the in-memory ASP.NET Core server is less performant in handling object results (with or without a Formatter attribute) than when handling IActionResults

## Future Research

Compare the Jil with JilFormatterAttribute, Jil with JsonActionResult, CSV, and byte[] endpoint performance using a non in-memory test infrastructure like the [github.com/aspnet/benchmarks](https://github.com/aspnet/benchmarks) project.

Discovery of better performance with Jil IActionResult scenario than JilFormatterAttribute scenario could yield an improved MvcJson score

