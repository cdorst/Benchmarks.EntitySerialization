using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Benchmarks
{
    [SimpleJob(10)]
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
        public byte[] ProtobufNet()
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, Constants.Entity);
                return stream.ToArray();
            }
        }
        [Benchmark]
        public byte[] MessagePackCSharp() => MessagePack.MessagePackSerializer.Serialize(Constants.Entity);

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
}
