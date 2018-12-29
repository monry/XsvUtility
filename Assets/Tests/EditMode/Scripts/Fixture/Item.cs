namespace Monry.XsvUtility.Fixture
{
    [XsvSerializable(SerializeMode.Indexed)]
    public struct ItemIndexed
    {
        [XsvColumn(0)]
        public string Hash { get; set; }
        [XsvColumn(1)]
        public int Size { get; set; }
    }

    [XsvSerializable(SerializeMode.Named)]
    public struct ItemNamed
    {
        [XsvColumn("hash")]
        public string Hash { get; set; }
        [XsvColumn("size")]
        public int Size { get; set; }
    }
}