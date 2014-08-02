using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSqlFlex.SqlParser;

namespace Spike
{
    class Program
    {
        static void Main(string[] args)
        {
            DoStuff();
        }

        static async void DoStuff()
        {
            var tokenizerTask = SqlTokenizer.TokenizeAsync("SELECT * FROM MyTable");
            var sqlTokens = await tokenizerTask;
            for (int i = 0; i < sqlTokens.Count; i += 1)
            {
                Debug.Print(sqlTokens[i].TokenType.ToString());
            }
        }
    }
}
