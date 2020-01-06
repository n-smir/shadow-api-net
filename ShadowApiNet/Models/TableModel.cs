using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ShadowApiNet.Models
{
    internal class TableModel
    {
        public Type Type { get; set; }
        public PropertyInfo[] Fields { get; set; }
        public PropertyInfo PK { get; set; }
    }
}
