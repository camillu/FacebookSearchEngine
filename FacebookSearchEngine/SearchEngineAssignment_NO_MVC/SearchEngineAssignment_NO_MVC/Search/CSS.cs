using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngineAssignment_NO_MVC.Search
{
    public class CSS
    {
        List<KeyValuePair<string, string>> rules = new List<KeyValuePair<string, string>>();

        public void AddRule(string property, string value)
        {
            rules.Add(new KeyValuePair<string,string>(property,value));
        }

        public string Generate()
        {
            string generated = null;
            foreach (KeyValuePair<string, string> kvp in rules)
            {
                generated += kvp.Key + ":" + kvp.Value + ";";
            }
            return generated;
        }
    }
}