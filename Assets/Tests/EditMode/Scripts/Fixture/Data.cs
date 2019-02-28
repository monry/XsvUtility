using System.Collections.Generic;

namespace Monry.XsvUtility.Fixture
{
    public struct DataIndexed
    {
        [XsvRow]
        public IEnumerable<ItemIndexed> ItemList { get; set; }
    }

    public struct DataNamed
    {
        [XsvRow]
        public IEnumerable<ItemNamed> ItemList { get; set; }
    }

    public struct DataSimpleEnum
    {
        [XsvRow]
        public IEnumerable<ItemSimpleEnum> ItemList { get; set; }
    }

    public struct DataFlagEnum
    {
        [XsvRow]
        public IEnumerable<ItemFlagEnum> ItemList { get; set; }
    }
}
