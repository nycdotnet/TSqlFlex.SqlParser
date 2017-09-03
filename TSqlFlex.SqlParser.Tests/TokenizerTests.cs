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
    class TokenizerGeneralSyntaxTests
    {
        [Test()]
        public async Task WhenPassedEmptyString_ReturnsEmptyArray()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("");
            var expected = new List<SqlToken>();
            Assert.AreEqual(expected, await actualTask);
        }

        [Test()]
        public async Task WhenPassedSingleSpace_ReturnsArrayWithOneWhitespaceToken()
        {
            var actualTask = SqlTokenizer.TokenizeAsync(" ");
            var expected = new List<SqlToken>();
            expected.Add (new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 1));
            expected[0].Text = " ";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task WhenPassedWhitespaceSeparatedByNewline_ReturnsWhitespaceSeparatedByNewline()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("  \n ");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 1));
            expected[0].Text = "  ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 3));
            expected[1].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 2, 1));
            expected[2].Text = " ";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task LineCommentWithNoBody_ReturnsNoBody()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("--");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentStart, 1, 1));
            expected[0].Text = "--";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task SimpleString_ReturnsNoBody()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("''");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 2));
            expected[1].Text = "'";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task SimpleStringWithBody_ReturnsTheBody()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("'test'");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 2));
            expected[1].Text = "test";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 6));
            expected[2].Text = "'";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task StringStartingWithEscapedQuote_ReturnsCorrectly()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("''' '");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 2));
            expected[1].Text = "' ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 5));
            expected[2].Text = "'";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task StringEndingWithEscapedQuote_ReturnsCorrectly()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("' '''");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 2));
            expected[1].Text = " '";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 5));
            expected[2].Text = "'";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }


        [Test()]
        public async Task MultilineString_ParsesCorrectly()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("'testline1\r\ntestline2'");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 1));
            expected[0].Text = "'";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 2));
            expected[1].Text = "testline1";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 11));
            expected[2].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 2, 1));
            expected[3].Text = "testline2";
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 2, 10));
            expected[4].Text = "'";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task WhenPassedWhitespaceThenLineComment_ReturnsCorrectResult()
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

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task SimpleBlockComment_ReturnsCorrectResult()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("/*test*/");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentStart, 1, 1));
            expected[0].Text = "/*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 1, 3));
            expected[1].Text = "test";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentEnd, 1, 7));
            expected[2].Text = "*/";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task BlockCommentWithInternalBlockCommentStart_ReturnsCorrectResult()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("/*\r\n/**/");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentStart, 1, 1));
            expected[0].Text = "/*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Newline, 1, 3));
            expected[1].Text = "\r\n";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentBody, 2, 1));
            expected[2].Text = "/*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.BlockCommentEnd, 2, 3));
            expected[3].Text = "*/";
            
            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task MultilineBlockComment_ReturnsCorrectResult()
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

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task JunkFollowedByWhitespaceAndLineComment_ReturnsCorrectResult()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("lijdfisuyndfk --this is junk");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 1));
            expected[0].Text = "lijdfisuyndfk";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 14));
            expected[1].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentStart, 1, 15));
            expected[2].Text = "--";
            expected.Add(new SqlToken(SqlToken.TokenTypes.LineCommentBody, 1, 17));
            expected[3].Text = "this is junk";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task RegularBracketizedTokens_ReturnCorrectlyResult()
        {
            int tokenIndex = 0;
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT 1 [z], 2 [yy]");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[tokenIndex].Text = "SELECT"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 8));
            expected[tokenIndex].Text = "1"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenBracket, 1, 10));
            expected[tokenIndex].Text = "["; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.BracketBody, 1, 11));
            expected[tokenIndex].Text = "z"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseBracket, 1, 12));
            expected[tokenIndex].Text = "]"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Comma, 1, 13));
            expected[tokenIndex].Text = ","; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 14));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 15));
            expected[tokenIndex].Text = "2"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 16));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenBracket, 1, 17));
            expected[tokenIndex].Text = "["; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.BracketBody, 1, 18));
            expected[tokenIndex].Text = "yy"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseBracket, 1, 20));
            expected[tokenIndex].Text = "]"; tokenIndex += 1;

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }


        [Test()]
        public async Task DoubleOpenBracketTokens_ReturnCorrectlyResult()
        {
            int tokenIndex = 0;
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT 1 [[z]");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[tokenIndex].Text = "SELECT"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 8));
            expected[tokenIndex].Text = "1"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenBracket, 1, 10));
            expected[tokenIndex].Text = "["; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.BracketBody, 1, 11));
            expected[tokenIndex].Text = "[z"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseBracket, 1, 13));
            expected[tokenIndex].Text = "]"; tokenIndex += 1;

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task MidExtraOpenBracketTokens_ReturnCorrectlyResult()
        {
            int tokenIndex = 0;
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT 1 [z[z]");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[tokenIndex].Text = "SELECT"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 8));
            expected[tokenIndex].Text = "1"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenBracket, 1, 10));
            expected[tokenIndex].Text = "["; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.BracketBody, 1, 11));
            expected[tokenIndex].Text = "z[z"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseBracket, 1, 14));
            expected[tokenIndex].Text = "]"; tokenIndex += 1;

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task EscaspedCloseBracketTokensAtStart_ReturnCorrectlyResult()
        {
            int tokenIndex = 0;
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT 1 []]z]");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[tokenIndex].Text = "SELECT"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 8));
            expected[tokenIndex].Text = "1"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenBracket, 1, 10));
            expected[tokenIndex].Text = "["; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.BracketBody, 1, 11));
            expected[tokenIndex].Text = "]z"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseBracket, 1, 14));
            expected[tokenIndex].Text = "]"; tokenIndex += 1;

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }
    }

    class TokenizerSqlStatementTests
    {
        [Test()]
        public async Task BasicSelectStarQuery_ReturnsSameTokensPlusUnknownForTableName()
        {
            var actualTask = SqlTokenizer.TokenizeAsync("SELECT * FROM MyTable");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[0].Text = "SELECT";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[1].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 8));
            expected[2].Text = "*";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 9));
            expected[3].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 10));
            expected[4].Text = "FROM";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 14));
            expected[5].Text = " ";
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 15));
            expected[6].Text = "MyTable";

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }

        [Test()]
        public async Task BasicInsertQuery_ReturnsSameTokensPlusUnknownForTableName()
        {
            int tokenIndex = 0;
            var actualTask = SqlTokenizer.TokenizeAsync("INSERT INTO MyTable (Field1, Field2) VALUES ('X', 123);");
            var expected = new List<SqlToken>();
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 1));
            expected[tokenIndex].Text = "INSERT"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 7));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 8));
            expected[tokenIndex].Text = "INTO"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 12));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 13));
            expected[tokenIndex].Text = "MyTable"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 20));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenParenthesis, 1, 21));
            expected[tokenIndex].Text = "("; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 22));
            expected[tokenIndex].Text = "Field1"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Comma, 1, 28));
            expected[tokenIndex].Text = ","; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 29));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 30));
            expected[tokenIndex].Text = "Field2"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseParenthesis, 1, 36));
            expected[tokenIndex].Text = ")"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 37));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Keyword, 1, 38));
            expected[tokenIndex].Text = "VALUES"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 44));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.OpenParenthesis, 1, 45));
            expected[tokenIndex].Text = "("; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringStart, 1, 46));
            expected[tokenIndex].Text = "'"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringBody, 1, 47));
            expected[tokenIndex].Text = "X"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.StringEnd, 1, 48));
            expected[tokenIndex].Text = "'"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Comma, 1, 49));
            expected[tokenIndex].Text = ","; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Whitespace, 1, 50));
            expected[tokenIndex].Text = " "; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Unknown, 1, 51));
            expected[tokenIndex].Text = "123"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.CloseParenthesis, 1, 54));
            expected[tokenIndex].Text = ")"; tokenIndex += 1;
            expected.Add(new SqlToken(SqlToken.TokenTypes.Semicolon, 1, 55));
            expected[tokenIndex].Text = ";"; tokenIndex += 1;

            Assert.That(await actualTask, Is.EquivalentTo(expected));
        }
    }
}
