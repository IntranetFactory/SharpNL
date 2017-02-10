using System.Collections.Generic;
using System.Text;

namespace ModelGeneratorW.Tools
{
    interface IIntentLineSplitter
    {
        string[] Split(string line, bool ignoreEmpty = false);
    }

    class IntentLineSplitter : IIntentLineSplitter
    {
        public IntentLineSplitter()
        {

        }

        public string[] Split(string line, bool ignoreEmpty = false)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(line))
            {
                return result.ToArray();
            }

            /* line is either like 
             * this: "is <employee>james</employee> in the office today?", or
             * this: U2,I want to Upload a video to the Corporate YouTube,"Videos, YouTube",1000083
             * " character opens a new scope where comma doesn't split
             * , charater closes a string
             */

            var sb = new StringBuilder();
            var closeWhenComma = true;
            var isQuoteChar = false;
            for (int i = 0; i < line.Length; i++)
            {
                var current = line[i];
                isQuoteChar = current == '"';
                if (isQuoteChar)
                {
                    closeWhenComma = !closeWhenComma; 
                }

                if (current != ',' || !closeWhenComma)
                {
                    sb.Append(current);
                }
                else if (current == ',' && closeWhenComma)
                {
                    if (sb.Length > 0 || !ignoreEmpty)
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                }
            }

            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
            }

            return result.ToArray();
        }
    }
}
