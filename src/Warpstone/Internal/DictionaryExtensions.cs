using System;
using System.Collections.Generic;
using System.Text;

namespace Warpstone.Internal
{
    public static class DictionaryExtensions
    {

        public static Dictionary<T1, T2> Modify<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 value)
        {
            Dictionary<T1, T2> copy = new Dictionary<T1, T2>(dict);
            copy[key] = value;
            return copy;
        }

        public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> dict1, Dictionary<T1, T2> dict2)
        {
            Dictionary<T1, T2> copy = new Dictionary<T1, T2>(dict1);

            foreach (KeyValuePair<T1, T2> entry in dict2)
            {
                copy[entry.Key] = entry.Value;
            }

            return copy;
        }
    }
}
