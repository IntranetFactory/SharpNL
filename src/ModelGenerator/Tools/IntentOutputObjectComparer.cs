using ModelGenerator.Models;
using System.Collections.Generic;

namespace ModelGenerator.Tools
{
    class IntentOutputObjectComparer : IComparer<IntentsOutputObject>
    {
        public int Compare(IntentsOutputObject x, IntentsOutputObject y)
        {
            var result = x.Name.Length - y.Name.Length;
            if (result == 0)
            {
                result = x.Name.CompareTo(y.Name);
            }
            return result;
        }
    }
}
