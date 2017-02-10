using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGeneratorW.Models
{
    public class AppOutputObject
    {
        public int Version { get; set; }
        public string ZipCommand { get; set; }
        public AppDataOutputObject Data { get; set; }
    }

    public class AppDataOutputObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
    }
}
