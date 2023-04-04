using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.Elastic
{
    public class ElasticOptions
    {
        public const string SectionName = "Elastic";

        public string Connection { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<Uri> Uris => Connection.Split('|').Select(x => new Uri(x)).ToList();
    }
}
