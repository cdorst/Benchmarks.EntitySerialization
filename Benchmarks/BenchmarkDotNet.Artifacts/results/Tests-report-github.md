``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300-rc1-008673
  [Host]     : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT
  Job-LUOAKF : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT

LaunchCount=10  

```
|                                   Method |          Mean |       Error |        StdDev |        Median | Rank |
|----------------------------------------- |--------------:|------------:|--------------:|--------------:|-----:|
|                     BlockCopyDeserialize |      42.34 ns |   0.2441 ns |     0.9027 ns |      42.31 ns |    9 |
|                BlockCopyDeserializeByRef |      42.03 ns |   0.2022 ns |     0.7352 ns |      42.02 ns |    8 |
|                       BlockCopySerialize |      22.55 ns |   0.1142 ns |     0.4180 ns |      22.56 ns |    3 |
|            ByteSerializeSpanBitConverter |      29.82 ns |   0.2318 ns |     0.8803 ns |      29.74 ns |    5 |
|          ByteSerializeSpanStructReadonly |      44.09 ns |   0.2607 ns |     0.9412 ns |      43.98 ns |   10 |
|              ByteSerializeReadOnlyMemory |      16.76 ns |   0.3013 ns |     1.0916 ns |      16.30 ns |    2 |
|                ByteSerializeReadOnlySpan |      15.28 ns |   0.0930 ns |     0.3405 ns |      15.21 ns |    1 |
|  ByteDeserializeReadOnlySpanBitConverter |      37.06 ns |   0.3088 ns |     1.1226 ns |      36.69 ns |    7 |
| ByteDeserializeReadOnlySpanBitRefStructs |      35.15 ns |   0.1331 ns |     0.4788 ns |      34.99 ns |    6 |
|                      BlockCopyMixedBytes |      25.65 ns |   0.1862 ns |     0.7162 ns |      25.65 ns |    4 |
|                         StringBuilderCsv |     122.41 ns |   0.6778 ns |     2.4382 ns |     122.29 ns |   12 |
|                         JilJsonSerialize |     405.32 ns |   2.1953 ns |     7.9533 ns |     405.08 ns |   14 |
|                       JilJsonDeserialize |     425.24 ns |   3.5032 ns |    12.8251 ns |     422.24 ns |   15 |
|                        StringBuilderJson |     431.59 ns |   2.3170 ns |     8.5985 ns |     430.90 ns |   16 |
|                           NewtonsoftJson |   1,420.23 ns |   6.4967 ns |    23.8658 ns |   1,418.10 ns |   17 |
|                              ProtobufNet |     330.28 ns |   1.9827 ns |     7.3083 ns |     329.99 ns |   13 |
|                        MessagePackCSharp |      76.13 ns |   1.2688 ns |     4.5643 ns |      76.72 ns |   11 |
|                 AspNetCoreApiDefaultJson | 114,786.54 ns | 500.7055 ns | 1,820.3811 ns | 114,700.95 ns |   24 |
|            AspNetCoreApiJilJsonFormatter | 112,423.19 ns | 409.3078 ns | 1,482.8857 ns | 112,271.83 ns |   23 |
|         AspNetCoreApiJilJsonActionResult | 100,674.24 ns | 523.2267 ns | 1,888.9243 ns | 100,633.11 ns |   21 |
|                         AspNetCoreApiCsv |  91,274.93 ns | 427.8149 ns | 1,571.5917 ns |  91,142.81 ns |   20 |
|                   AspNetCoreApiByteArray | 105,828.85 ns | 387.7865 ns | 1,404.9159 ns | 105,952.85 ns |   22 |
|       AspNetCoreApiByteArrayActionResult |  88,801.22 ns | 434.2339 ns | 1,643.5558 ns |  88,740.05 ns |   19 |
|           AspNetCoreApiReadOnlyMemoryMvc |  88,833.91 ns | 374.0342 ns | 1,364.5926 ns |  88,830.05 ns |   19 |
|        AspNetCoreApiReadOnlyMemoryRouter |  72,064.47 ns | 330.4402 ns | 1,197.1556 ns |  72,125.50 ns |   18 |
