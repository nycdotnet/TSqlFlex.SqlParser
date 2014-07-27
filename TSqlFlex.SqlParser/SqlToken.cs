using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser
{
    public class SqlToken
    {
        public enum TokenTypes
        {
            Whitespace,
            Keyword,
            Newline,
            BlockCommentStart,
            BlockCommentBody,
            BlockCommentEnd,
            LineCommentStart,
            LineCommentBody,
            Unknown
        }
        public TokenTypes TokenType { get; set; }
        /// <summary>
        /// One-based line number
        /// </summary>
        public int LineNumber { get; set; }
        /// <summary>
        /// One-based character start index (starting from left)
        /// </summary>
        public int StartCharacterIndex { get; set; }
        public int Length { get { return Text.Length;  } }
        public string Text { get; set; }

        static public IList<SqlToken> ExtractTokens(Char[] charsToEvalue, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            int charIndex = oneBasedStartCharacterIndex;
            SqlToken currentToken;
            if (isWhitespace(charsToEvalue))
            {
                currentToken = new SqlToken(TokenTypes.Whitespace, oneBasedLineNumber, oneBasedStartCharacterIndex);
                for (int i = 1; i < charsToEvalue.Length; i += 1)
                {
                    if (!isWhitespace(charsToEvalue[i]))
                    {
                        currentToken.Text = new String(charsToEvalue, 0, i);
                        tokens.Add(currentToken);
                        return tokens;
                    }
                }
                currentToken.Text = new String(charsToEvalue);
                tokens.Add(currentToken);
                return tokens;
            }
            else if (isLineCommentStart(charsToEvalue))
            {
                currentToken = new SqlToken(TokenTypes.LineCommentStart, oneBasedLineNumber, oneBasedStartCharacterIndex);
                currentToken.Text = "--";
                tokens.Add(currentToken);
                currentToken = new SqlToken(TokenTypes.LineCommentBody, oneBasedLineNumber, oneBasedStartCharacterIndex + 2);
                currentToken.Text = charsToEvalue.Skip(2).ToString();
                tokens.Add(currentToken);
                return tokens;
            }
            currentToken = new SqlToken(TokenTypes.Unknown, oneBasedLineNumber, oneBasedStartCharacterIndex);
            currentToken.Text = charsToEvalue.ToString();
            tokens.Add(currentToken);
            return tokens;
        }

        static private bool isWhitespace(Char theChar) {
            return (theChar == ' ' || theChar == '\t');
        }
        static private bool isWhitespace(Char[] theCharArray)
        {
            return isWhitespace(theCharArray[0]);
        }
        static private bool isLineCommentStart(Char[] theCharArray)
        {
            return (theCharArray[0] == '-' && theCharArray[1] == '-');
        }


        public SqlToken(TokenTypes type, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            StartCharacterIndex = oneBasedStartCharacterIndex;
            LineNumber = oneBasedLineNumber;
            TokenType = type;
        }
    }
}
