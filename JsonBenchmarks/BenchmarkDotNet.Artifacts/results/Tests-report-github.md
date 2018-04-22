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
