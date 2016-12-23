using System.Collections.Generic;

namespace ModelGenerator.Models
{
    sealed class EntityOutputObject
    {
        public string Value { get; set; }
        public string[] Synonyms { get; set; }
    }

    sealed class EntitiesOutputObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOverridable { get; set; }

        public List<EntityOutputObject> Entries { get; set; }

        public bool IsEnum { get; set; }
        public bool AutomatedExpansion { get; set; }

        public EntitiesOutputObject()
        {
            IsOverridable = true;
            IsEnum = false;
            AutomatedExpansion = true;
        }
    }
}
