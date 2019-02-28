using System;

namespace Monry.XsvUtility
{
    // Primary key
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class XsvKeyAttribute : Attribute
    {
    }
}