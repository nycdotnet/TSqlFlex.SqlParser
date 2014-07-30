using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using TSqlFlex.SqlParser;


namespace TSqlFlex.SqlParser.Tests
{
    [TestFixture()]
    class TokenizerTests
    {
        [Test()]
        public async void WhenPassedEmptyString_ReturnsEmptyArray()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("");
            var expected = new List<SqlToken>();
            var actual = await actualTask;
            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public async void WhenPassedSingleSpace_ReturnsArrayWithOneWhitespaceToken()
        {
            var actualTask = SqlTokenizer.TokenizeAsync(" ");
            var expected = new List<SqlToken>();
            expected.Add (new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 1));
            expected[0].Text = " ";
            var actual = await actualTask;
            AssertArePropertiesEqual(expected, actual);
        }

        [Test()]
        public async void WhenPassedWhitespaceSeparatedByNewline_ReturnsWhitespaceSeparatedByNewline()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("  \n ");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 1));
            expected[0].Text = "  ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 3));
            expected[1].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 2, 1));
            expected[2].Text = " ";
            var actual = await actualTask;

            AssertArePropertiesEqual(expected, actual);
        }

        [Test()]
        public async void LineCommentWithNoBody_ReturnsNoBody()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("--");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentStart, 1, 1));
            expected[0].Text = "--";
            var actual = await actualTask;

            AssertArePropertiesEqual(expected, actual);
        }

        [Test()]
        public async void WhenPassedWhitespaceThenLineComment_ReturnsCorrectResult()
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

            AssertArePropertiesEqual(expected, actual);
        }

        [Test()]
        public async void SimpleBlockComment_ReturnsCorrectResult()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("/*test*/");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentStart, 1, 1));
            expected[0].Text = "/*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 1, 3));
            expected[1].Text = "test";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentEnd, 1, 7));
            expected[2].Text = "*/";
            var actual = await actualTask;

            AssertArePropertiesEqual(expected, actual);
        }

        [Test()]
        public async void MultilineBlockComment_ReturnsCorrectResult()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("/* \n test\n */ ");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentStart, 1, 1));
            expected[0].Text = "/*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 1, 3));
            expected[1].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 4));
            expected[2].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 2, 1));
            expected[3].Text = " test";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 2, 6));
            expected[4].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 3, 1));
            expected[5].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentEnd, 3, 2));
            expected[6].Text = "*/";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 3, 4));
            expected[7].Text = " ";
            var actual = await actualTask;

            AssertArePropertiesEqual(expected, actual);
        }


        //Thanks: http://stackoverflow.com/questions/318210/compare-equality-between-two-objects-in-nunit/
        public static void AssertArePropertiesEqual(object expected, object actual)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var expectedJson = serializer.Serialize(expected);
            var actualJson = serializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson, "expected: " + expectedJson + "\r\n" + "  actual:   " + actualJson);
        }
    }
}
