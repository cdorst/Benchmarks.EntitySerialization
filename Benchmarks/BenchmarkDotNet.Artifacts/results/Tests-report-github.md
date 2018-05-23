``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300-rc1-008673
  [Host]     : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT
  Job-BHDFIP : .NET Core 2.1.0-rc1 (CoreCLR 4.6.26426.02, CoreFX 4.6.26426.04), 64bit RyuJIT

LaunchCount=10  

```
|                                   Method |          Mean |       Error |        StdDev |        Median | Rank |  Gen 0 | Allocated |
|----------------------------------------- |--------------:|------------:|--------------:|--------------:|-----:|-------:|----------:|
|                     BlockCopyDeserialize |      42.12 ns |   0.2810 ns |     1.0287 ns |      42.00 ns |    8 | 0.0356 |     112 B |
|                BlockCopyDeserializeByRef |      41.97 ns |   0.2713 ns |     0.9761 ns |      41.78 ns |    8 | 0.0356 |     112 B |
|                       BlockCopySerialize |      22.29 ns |   0.2173 ns |     0.7928 ns |      22.10 ns |    3 | 0.0203 |      64 B |
|            ByteSerializeSpanBitConverter |      30.36 ns |   0.2618 ns |     1.1534 ns |      30.21 ns |    5 | 0.0305 |      96 B |
|          ByteSerializeSpanStructReadonly |      43.95 ns |   0.2895 ns |     1.0562 ns |      43.72 ns |    9 | 0.0305 |      96 B |
|              ByteSerializeReadOnlyMemory |      16.35 ns |   0.0879 ns |     0.3186 ns |      16.31 ns |    2 | 0.0102 |      32 B |
|                ByteSerializeReadOnlySpan |      15.49 ns |   0.1359 ns |     0.5160 ns |      15.32 ns |    1 | 0.0102 |      32 B |
|  ByteDeserializeReadOnlySpanBitConverter |      37.32 ns |   0.2409 ns |     0.8728 ns |      37.15 ns |    7 | 0.0356 |     112 B |
| ByteDeserializeReadOnlySpanBitRefStructs |      35.88 ns |   0.1776 ns |     0.6614 ns |      35.76 ns |    6 | 0.0153 |      48 B |
|                   MixedKeyTypesBlockCopy |      25.83 ns |   0.1834 ns |     0.6805 ns |      25.82 ns |    4 | 0.0356 |     112 B |
|                        MixedKeyTypesSpan |      42.00 ns |   0.2012 ns |     0.7262 ns |      41.95 ns |    8 | 0.0127 |      40 B |
|                         StringBuilderCsv |     121.82 ns |   0.6861 ns |     2.6467 ns |     121.37 ns |   11 | 0.0508 |     160 B |
|                         JilJsonSerialize |     398.28 ns |   2.2293 ns |     8.1612 ns |     397.39 ns |   13 | 0.2594 |     816 B |
|                       JilJsonDeserialize |     427.69 ns |   3.1554 ns |    11.5915 ns |     428.34 ns |   14 | 0.0458 |     144 B |
|                        StringBuilderJson |     434.97 ns |   2.5303 ns |    10.6302 ns |     435.38 ns |   15 | 0.2542 |     800 B |
|                           NewtonsoftJson |   1,399.09 ns |   7.5792 ns |    28.3148 ns |   1,392.33 ns |   16 | 0.5150 |    1624 B |
|                              ProtobufNet |     326.17 ns |   1.7980 ns |     6.7832 ns |     326.03 ns |   12 | 0.1702 |     536 B |
|                        MessagePackCSharp |      72.28 ns |   0.5171 ns |     1.9061 ns |      72.00 ns |   10 | 0.0126 |      40 B |
|                 AspNetCoreApiDefaultJson | 115,827.86 ns | 528.7379 ns | 1,908.8205 ns | 115,761.51 ns |   24 | 3.9063 |    5512 B |
|            AspNetCoreApiJilJsonFormatter | 114,701.84 ns | 524.6135 ns | 1,979.2290 ns | 114,571.80 ns |   23 | 3.7842 |    5536 B |
|         AspNetCoreApiJilJsonActionResult | 102,398.60 ns | 403.6869 ns | 1,488.0215 ns | 102,229.11 ns |   21 | 3.9063 |    5552 B |
|                         AspNetCoreApiCsv |  93,839.51 ns | 508.7330 ns | 1,849.5661 ns |  93,762.68 ns |   20 | 3.4180 |    5448 B |
|                   AspNetCoreApiByteArray | 107,763.87 ns | 393.4655 ns | 1,405.2910 ns | 107,954.99 ns |   22 | 3.7842 |    5456 B |
|       AspNetCoreApiByteArrayActionResult |  91,049.77 ns | 355.9585 ns | 1,285.0619 ns |  90,881.38 ns |   18 | 3.4180 |    5568 B |
|           AspNetCoreApiReadOnlyMemoryMvc |  91,604.79 ns | 422.9263 ns | 1,542.9657 ns |  91,481.84 ns |   19 | 3.4180 |    5576 B |
|        AspNetCoreApiReadOnlyMemoryRouter |  74,332.75 ns | 314.6925 ns | 1,123.9474 ns |  74,352.01 ns |   17 | 2.6855 |    5416 B |
