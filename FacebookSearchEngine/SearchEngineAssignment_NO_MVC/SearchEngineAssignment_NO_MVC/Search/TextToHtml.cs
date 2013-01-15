using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngineAssignment_NO_MVC.Search
{
    public class TextToHtml
    {
        public static string ToParagraph(string source)
        {
            return "<p>" + source + "</p>";
        }
    }
}