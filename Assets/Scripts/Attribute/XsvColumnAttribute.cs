using System;

namespace Monry.XsvUtility
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XsvColumnAttribute : Attribute
    {
        public int Index { get; }

        public string Name { get; }

        public XsvColumnAttribute(int index)
        {
            Index = index;
        }

        public XsvColumnAttribute(string name)
        {
            Name = name;
        }
    }
}