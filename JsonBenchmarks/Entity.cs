using Common.Extensions.Memory;
using System;
using System.Text;
using static Common.Extensions.Memory.IntegerReadOnlyMemoryMapper;
using static Common.Extensions.Memory.IntegerReadOnlySpanMapper;

namespace JsonBenchmarks
{
    public class Entity
    {
        public int EntityId { get; set; } = 1_000_000;

        public Foo ForeignKeyOne { get; set; } // Assume that reference objects are not loaded
        public int ForeignKeyOneId { get; set; } = 1_000_001;

        public Foo ForeignKeyTwo { get; set; } // Assume that reference objects are not loaded
        public int ForeignKeyTwoId { get; set; } = 1_000_002;

        public override string ToString()
            => new StringBuilder().Append("{\"entityId\":").Append(EntityId).Append(",\"foreignKeyOneId\":").Append(ForeignKeyOneId).Append(",\"foreignKeyTwoId\":").Append(ForeignKeyTwoId).Append(',').ToString();

        /// <summary>
        /// Special-case to be used as a plaintext response in an ASP.NET Core API environment;
        /// Assumes:
        ///     Requesting client knows how to construct object from the resulting string
        ///     Requesting client already knows the primary-key value
        /// </summary>
        public string ToStringCsv()
            => new StringBuilder().Append(ForeignKeyOneId).Append(',').Append(ForeignKeyTwoId).ToString();

        public static Entity FromBytes(byte[] bytes, int entityId = 0)
        {
            var foreignKeyOneIdBytes = new byte[4];
            Buffer.BlockCopy(bytes, 0, foreignKeyOneIdBytes, 0, 4);
            var foreignKeyTwoIdBytes = new byte[4];
            Buffer.BlockCopy(bytes, 4, foreignKeyTwoIdBytes, 0, 4);
            return new Entity
            {
                EntityId = entityId,
                ForeignKeyOneId = BitConverter.ToInt32(foreignKeyOneIdBytes, 0),
                ForeignKeyTwoId = BitConverter.ToInt32(foreignKeyTwoIdBytes, 0)
            };
        }

        public static Entity FromBytesInRef(in byte[] bytes, in int entityId = 0)
        {
            var foreignKeyOneIdBytes = new byte[4];
            Buffer.BlockCopy(bytes, 0, foreignKeyOneIdBytes, 0, 4);
            var foreignKeyTwoIdBytes = new byte[4];
            Buffer.BlockCopy(bytes, 4, foreignKeyTwoIdBytes, 0, 4);
            return new Entity
            {
                EntityId = entityId,
                ForeignKeyOneId = BitConverter.ToInt32(foreignKeyOneIdBytes, 0),
                ForeignKeyTwoId = BitConverter.ToInt32(foreignKeyTwoIdBytes, 0)
            };
        }

        public byte[] ToBytes()
        {
            var result = new byte[8];
            Buffer.BlockCopy(new[] { ForeignKeyOneId, ForeignKeyTwoId }, 0, result, 0, 8);
            return result;
        }

        public Span<byte> ToBytesSpanBitConverter()
        {
            Span<byte> result = new byte[8];
            byte i = 0;
            foreach (var @byte in BitConverter.GetBytes(ForeignKeyOneId))
            {
                result[i] = @byte;
                i++;
            }
            foreach (var @byte in BitConverter.GetBytes(ForeignKeyTwoId))
            {
                result[i] = @byte;
                i++;
            }
            return result;
        }

        public Span<byte> ToBytesSpanStruct()
        {
            var key1 = ForeignKeyOneId.ToSpan();
            var key2 = ForeignKeyTwoId.ToSpan();
            return new byte[8]
            {
                key1[0],
                key1[1],
                key1[2],
                key1[3],
                key2[0],
                key2[1],
                key2[2],
                key2[3]
            };
        }

        public ReadOnlyMemory<byte> ToBytesReadonlyMemory()
            => MapMemory(ForeignKeyOneId, ForeignKeyTwoId);

        public ReadOnlySpan<byte> ToBytesReadonlySpan()
            => MapSpan(ForeignKeyOneId, ForeignKeyTwoId);
    }

    public class MixedKeyTypes
    {
        public static MixedKeyTypes Default = new MixedKeyTypes();

        public long ForeignKeyOneId { get; set; } = 1_000_000_000_000;
        public int ForeignKeyTwoId { get; set; } = 1_000_000;
        public short ForeignKeyThreeId { get; set; } = 1_000;
        public byte ForeignKeyFourId { get; set; } = 100;

        public byte[] ToBytes()
        {
            var result = new byte[32];
            Buffer.BlockCopy(new[] { ForeignKeyOneId, ForeignKeyTwoId, ForeignKeyThreeId, ForeignKeyFourId }, 0, result, 0, 32);
            return result;
        }
    }

    public class Foo
    {
        public int FooId { get; set; }
        public string MyProperty { get; set; }
    }
}
