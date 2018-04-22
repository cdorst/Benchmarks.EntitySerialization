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
