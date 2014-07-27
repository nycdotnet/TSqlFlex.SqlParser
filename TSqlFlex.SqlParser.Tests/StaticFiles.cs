using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser.Tests
{
    public class StaticFiles
    {
        static public string SimpleSelect()
        {
            return GetResourceByName("TSqlFlex.SqlParser.Tests.SqlScripts.SimpleSelect.sql");
        }

        static public string ComentsStringsAndWhitespace()
        {
            return GetResourceByName("TSqlFlex.SqlParser.Tests.SqlScripts.CommentsStringsAndWhitespace.sql");
        }

        static private string GetResourceByName(string resourceName)
        {
            string result;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
