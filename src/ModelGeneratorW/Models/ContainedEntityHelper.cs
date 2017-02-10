namespace ModelGeneratorW.Models
{
    public class ContainedEntityHelper
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ContainedEntityHelper()
        {
            Name = string.Empty;
            Value = string.Empty;
        }

        public ContainedEntityHelper(string name)
        {
            Name = name;
            Value = string.Empty;
        }

        public ContainedEntityHelper(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
