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
