using System;
using System.Collections.Generic;
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
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT * FROM MyTable");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Select, 1, 1));
            expected[0].Text = "SELECT";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[1].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Star, 1, 8));
            expected[2].Text = "*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[3].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Select, 1, 10));
            expected[4].Text = "FROM";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 14));
            expected[5].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 15));
            expected[6].Text = "MyTable";

            var actual = await actualTask;
            //Assert.AreEqual(expected, actual);
            Console.Write(actual[0].StartCharacterIndex);
            //Assert.AreEqual(expected, actual);
            /*Assert.AreEqual(expected[0].TokenType, actual[0].TokenType);
            Assert.AreEqual(expected[0].LineNumber, actual[0].LineNumber);
            Assert.AreEqual(expected[0].StartCharacterIndex, actual[0].StartCharacterIndex);
            Assert.AreEqual(expected[0].Length, actual[0].Length);
            Assert.AreEqual(expected[0].Text, actual[0].Text);
             * */
        }
    }
}
