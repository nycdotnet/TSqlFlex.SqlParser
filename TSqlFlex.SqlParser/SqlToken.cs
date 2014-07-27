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
            BlockComment,
            BlockCommentEnd,
            LineCommentStart,
            LineComment,
            Unknown
        }
        public TokenTypes TokenType { get; set; }
        public int LineNumber { get; set; }
        public int StartCharacterIndex { get; set; }
        public int Length { get { return Text.Length;  } }
        public string Text { get; set; }
        public SqlToken(Char[] charsToEvalue, int oneBasedStartCharacterIndex, int lineNumber)
        {
            StartCharacterIndex = oneBasedStartCharacterIndex;
            LineNumber = lineNumber;
            if (isWhitespace(charsToEvalue[0]))
            {
                TokenType = TokenTypes.Whitespace;
                for (int i = 1; i < charsToEvalue.Length; i+= 1)
                {
                    if (!isWhitespace(charsToEvalue[i]))
                    {
                        Text = new String(charsToEvalue, 0, i);
                        return;
                    }
                }
                Text = new String(charsToEvalue);
                return;
            }
            TokenType = TokenTypes.Unknown;
        }

        private bool isWhitespace(Char theChar) {
            return (theChar == ' ' || theChar == '\t');
        }

        public SqlToken(TokenTypes type, int startCharacterIndex, int lineNumber)
        {
            StartCharacterIndex = startCharacterIndex;
            LineNumber = lineNumber;
            TokenType = type;
        }
    }
}
