using System;
using TSqlFlex.SqlParser;

namespace Spike
{
    class Program
    {
        static void Main(string[] args)
        {
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
