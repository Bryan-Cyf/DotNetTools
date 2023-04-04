using System;
using System.Collections.Generic;

namespace Tools.Elastic
{
    public class MappingIndex
    {
        public Type Type { get; set; }

        public string IndexName { get; set; }

        public List<MappingColumn> Columns { get; } = new List<MappingColumn>(0);
    }
}