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
        /// <summary>
        /// Indicates if the block comment start token or string start token isn't known to be closed such as /* or '
        /// </summary>
        public bool IsOpen { get; set; }

        static public IList<SqlToken> ExtractTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex, SqlToken openTokenIfAny)
        {
            if (openTokenIfAny != null)
            {
                if (openTokenIfAny.TokenType == TokenTypes.BlockCommentStart)
                {
                    return ExtractBlockCommentTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
                }
            }
            else if (isWhitespace(charsToEvaluate))
            {
                return ExtractWhitespaceTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
            }
            else if (isLineCommentStart(charsToEvaluate))
            {
                return ExtractLineCommentTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
            }
            else if (isBlockCommentStart(charsToEvaluate))
            {
                return ExtractBlockCommentTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
            }
            return ExtractUnknownToken(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
        }

        private static IList<SqlToken> ExtractUnknownToken(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            
            var t = new SqlToken(TokenTypes.Unknown, oneBasedLineNumber, oneBasedStartCharacterIndex);
            t.Text = new String(charsToEvaluate);
            tokens.Add(t);
            return tokens;
        }

        private static IList<SqlToken> ExtractBlockCommentTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            const string BLOCK_COMMENT_START = "/*";
            const string BLOCK_COMMENT_END = "*/";
            List<SqlToken> tokens = new List<SqlToken>();

            int offset = 0;
            SqlToken t;

            if (isBlockCommentStart(charsToEvaluate)) { 
                t = new SqlToken(TokenTypes.BlockCommentStart, oneBasedLineNumber, oneBasedStartCharacterIndex);
                t.Text = BLOCK_COMMENT_START;
                offset += BLOCK_COMMENT_START.Length;
                tokens.Add(t);
            }

            if (charsToEvaluate.Length > offset)
            {
                for (int i = offset; i < charsToEvaluate.Length - 1; i += 1)
                {
                    if (isBlockCommentEnd(charsToEvaluate, i))
                    {
                        t = new SqlToken(TokenTypes.BlockCommentBody, oneBasedLineNumber, oneBasedStartCharacterIndex + offset);
                        t.Text = new String(charsToEvaluate, offset, i - BLOCK_COMMENT_END.Length);
                        tokens.Add(t);
                        t = new SqlToken(TokenTypes.BlockCommentEnd, oneBasedLineNumber, i + 1);
                        t.Text = BLOCK_COMMENT_END;
                        tokens.Add(t);
                        return tokens;
                    }
                }
                //We didn't find a block comment end (*/)

                //hack: fix this.
                var blockCommentStart = tokens.Where(tok => tok.TokenType == TokenTypes.BlockCommentStart);
                foreach (var bcs in blockCommentStart)
                {
                    bcs.IsOpen = true;
                }
                
                t = new SqlToken(TokenTypes.BlockCommentBody, oneBasedLineNumber, oneBasedStartCharacterIndex + offset);
                t.Text = new String(charsToEvaluate, offset, charsToEvaluate.Length - offset);
                tokens.Add(t);
                return tokens;
            }

            //hack: fix this.
            var blockCommentStart1 = tokens.Where(tok => tok.TokenType == TokenTypes.BlockCommentStart);
            foreach (var bcs in blockCommentStart1)
            {
                bcs.IsOpen = true;
            }
            return tokens;
        }

        private static IList<SqlToken> ExtractLineCommentTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            SqlToken t;
            const int LINE_COMMENT_TOKEN_LENGTH = 2;
            t = new SqlToken(TokenTypes.LineCommentStart, oneBasedLineNumber, oneBasedStartCharacterIndex);
            t.Text = "--";
            tokens.Add(t);

            if (charsToEvaluate.Length > LINE_COMMENT_TOKEN_LENGTH)
            {
                t = new SqlToken(TokenTypes.LineCommentBody, oneBasedLineNumber, oneBasedStartCharacterIndex + LINE_COMMENT_TOKEN_LENGTH);
                t.Text = new String(charsToEvaluate, 2, charsToEvaluate.Length - LINE_COMMENT_TOKEN_LENGTH);
                tokens.Add(t);
            }
            return tokens;
        }

        private static IList<SqlToken> ExtractWhitespaceTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            var t = new SqlToken(TokenTypes.Whitespace, oneBasedLineNumber, oneBasedStartCharacterIndex);
            for (int i = 1; i < charsToEvaluate.Length; i += 1)
            {
                if (!isWhitespace(charsToEvaluate[i]))
                {
                    t.Text = new String(charsToEvaluate, 0, i);
                    tokens.Add(t);
                    return tokens;
                }
            }
            t.Text = new String(charsToEvaluate);
            tokens.Add(t);
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
        static private bool isBlockCommentStart(Char[] theCharArray)
        {
            return (theCharArray[0] == '/' && theCharArray[1] == '*');
        }
        static private bool isBlockCommentEnd(Char[] theCharArray, int firstCharIndex)
        {
            return (theCharArray[firstCharIndex] == '*' && theCharArray[firstCharIndex + 1] == '/');
        }


        public SqlToken(TokenTypes type, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            StartCharacterIndex = oneBasedStartCharacterIndex;
            LineNumber = oneBasedLineNumber;
            TokenType = type;
            IsOpen = false;
        }
    }
}
