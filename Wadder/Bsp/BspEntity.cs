using System;
using System.Collections.Generic;

namespace Wadder.Bsp
{
    public class BspEntity
    {
        public Dictionary<string, string> Values { get; set; }

        public BspEntity()
        {
            Values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public string Get(string key)
        {
            return Values.ContainsKey(key) ? Values[key] : "";
        }
    }
}