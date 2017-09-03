using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSqlFlex.SqlParser;

namespace ConsoleExample.DotNet462
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Welcome to TSqlFlex.SqlParser running on .NET 4.6.2");
            ParseSql("SELECT * FROM MyTable");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static async void ParseSql(string sql)
        {
            var sqlTokens = await SqlTokenizer.TokenizeAsync(sql);
            Console.WriteLine($"Found {sqlTokens.Count} token(s) in `{sql}`.");
            foreach (var token in sqlTokens)
            {
                Console.WriteLine($"{token.TokenType.ToString()}: `{token.Text}`");
            }
        }
    }
}
