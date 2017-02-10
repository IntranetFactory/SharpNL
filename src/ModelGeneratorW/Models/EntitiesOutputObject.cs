using System.Collections.Generic;
using System.ComponentModel;

namespace ModelGeneratorW.Models
{
    sealed class EntityOutputObject
    {
        public string Value { get; set; }
        public List<string> Expressions { get; set; }

        public EntityOutputObject()
        {
            Expressions = new List<string>();
        }
    }

    sealed class EntitiesDataOutputObject
    {
        public List<string> Lookups { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        [DefaultValue(true)]
        public bool Exotic { get; set; }
        public string Id { get; set; }

        public List<EntityOutputObject> Values { get; set; }

        public string Doc { get; set; }
        [DefaultValue(true)]
        public bool Builtin { get; set; }

        public EntitiesDataOutputObject(string name)
        {
            Lookups = new List<string>() { "free-text", "keywords" };
            Name = name;
            Lang = "en";
            Exotic = false;
            Id = name;

            Values = new List<EntityOutputObject>();

            Doc = "User-defined entity";
            Builtin = false;
        }

    }

    sealed class EntitiesOutputObject
    {
        public EntitiesDataOutputObject Data { get; set; }

        public EntitiesOutputObject(string name)
        {
            Data = new EntitiesDataOutputObject(name);
        }
    }
}
