using System;
using System.Text;

namespace ModelGeneratorW.Tools
{
    interface IEntityParser
    {
        EntityParserResult Parse(string input);
    }


    class EntityParserResult
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }

    class EntityParser : IEntityParser
    {
        public EntityParserResult Parse(string input)
        {
            EntityParserResult result = new EntityParserResult();

            result.HasError = input.Length == 0;
            if (result.HasError)
            {
                result.ErrorMessage = "Input string is empty";
                return result;
            }

            string startTagName = string.Empty;
            string endTagName = string.Empty;
            string tagValue = string.Empty;
            bool hasEndTag = false;

            StringBuilder sb = new StringBuilder();
            int i;
            for (i = 0; i < input.Length; i++)
            {
                var current = input[i];
                char previous = i > 0 ? input[i - 1] : Char.MinValue;

                if (current == '<')
                {
                    if (!string.IsNullOrEmpty(startTagName))
                    {
                        tagValue = sb.ToString();
                        sb.Clear();
                    }
                }
                else if (current == '/' && previous == '<')
                {
                    hasEndTag = true;
                }
                else if (current == '>')
                {
                    if (startTagName.Equals(string.Empty))
                    {
                        startTagName = sb.ToString();
                    }
                    else
                    {
                        endTagName = sb.ToString();
                    }
                    sb.Clear();
                }
                else
                {
                    sb.Append(current);
                }
            }

            if (i == input.Length && !hasEndTag)
            {
                tagValue = sb.ToString();
            }

            result.Name = startTagName;
            result.Value = tagValue;

            result.HasError = string.IsNullOrEmpty(startTagName);
            if (result.HasError)
            {
                result.ErrorMessage = "Missing start tag";
                return result;
            }

            result.HasError = !hasEndTag;
            if (result.HasError)
            {
                result.ErrorMessage = string.Format("Missing end tag </{0}>", startTagName);
                return result;
            }

            result.HasError = !startTagName.Equals(endTagName);
            if (result.HasError)
            {
                result.ErrorMessage = string.Format("Tag names do not match. Expected <{0}> to match </{1}>");
                return result;
            }

            return result;
        }
    }
}
