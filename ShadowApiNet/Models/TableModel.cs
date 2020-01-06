using System;
using System.Reflection;

namespace ShadowApiNet.Models
{
    internal class TableModel
    {
        public Type Type { get; set; }
        public PropertyInfo[] Fields { get; set; }
        public PropertyInfo PK { get; set; }
    }
}
