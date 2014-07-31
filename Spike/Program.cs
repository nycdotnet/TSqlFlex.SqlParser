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
            var actualTask = SqlTokenizer.TokenizeAsync("''' '");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 2));
            expected[1].Text = "' ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 5));
            expected[2].Text = "'";

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
