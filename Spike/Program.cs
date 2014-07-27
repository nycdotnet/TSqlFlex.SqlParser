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
            var actualTask = SqlTokenizer.TokenizeAsync("   --This is a comment\n ");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 1));
            expected[0].Text = "   ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentStart, 1, 4));
            expected[1].Text = "--";
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentBody, 1, 6));
            expected[2].Text = "This is a comment";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 23));
            expected[3].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 2, 1));
            expected[4].Text = " ";
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
