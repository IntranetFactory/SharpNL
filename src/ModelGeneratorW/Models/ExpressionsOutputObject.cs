using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGeneratorW.Models
{
    public class ExpressionsOutputObject
    {
        public List<ExpressionOutputObject> Data { get; set; }

        public ExpressionsOutputObject()
        {
            Data = new List<ExpressionOutputObject>();
        }
    }

    public class ExpressionOutputObject
    {
        public string Text { get; set; }
        public List<ExpressionEntityEntryOutputObject> Entities { get; set; }

        public ExpressionOutputObject()
        {
            Entities = new List<ExpressionEntityEntryOutputObject>();
        }
    }

    public class ExpressionEntityEntryOutputObject
    {
        public string Entity { get; set; }
        public string Value { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public ExpressionEntityEntryOutputObject()
        {
            Entity = string.Empty;
            Value = string.Empty;
        }

        public ExpressionEntityEntryOutputObject(string name) : this()
        {
            Entity = name;
        }

        public ExpressionEntityEntryOutputObject(string name, string value) : this(name)
        {
            Value = value;
        }
    }
}
