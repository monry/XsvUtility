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
}