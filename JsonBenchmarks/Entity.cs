using System;
using System.Text;

namespace JsonBenchmarks
{
    public class Entity
    {
        public static Entity Default = new Entity();

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

        public byte[] ToBytes()
        {
            var result = new byte[8];
            Buffer.BlockCopy(new[] { ForeignKeyOneId, ForeignKeyTwoId }, 0, result, 0, 8);
            return result;
        }
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
