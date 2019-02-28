using System;

namespace Monry.XsvUtility
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XsvRowAttribute : Attribute
    {
    }
}