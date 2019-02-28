using System;

namespace Monry.XsvUtility.Fixture
{
    public enum SimpleEnum
    {
        Foo = 1,
        Bar = 2,
        Baz = 3,
        Quz = 4,
    }

    [Flags]
    public enum FlagEnum
    {
        Foo = 1 << 0,
        Bar = 1 << 1,
        Baz = 1 << 2,
        Quz = 1 << 3,
    }

    public struct ItemIndexed
    {
        [XsvColumn(0)]
        public string Hash { get; set; }
        [XsvColumn(1)]
        public int Size { get; set; }
    }

    public struct ItemNamed
    {
        [XsvColumn("hash")]
        public string Hash { get; set; }
        [XsvColumn("size")]
        public int Size { get; set; }
    }

    public struct ItemSimpleEnum
    {
        [XsvColumn(0)]
        public SimpleEnum SimpleEnum { get; set; }
        [XsvColumn(1)]
        public int Size { get; set; }
    }

    public struct ItemFlagEnum
    {
        [XsvColumn(0)]
        public FlagEnum FlagEnum { get; set; }
        [XsvColumn(1)]
        public int Size { get; set; }
    }
}
