﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSqlFlex.SqlParser
{
    public class SqlToken
    {

        public const string BLOCK_COMMENT_START = "/*";
        public const string BLOCK_COMMENT_END = "*/";
        public const string LINE_COMMENT_TOKEN = "--";
        public const string QUOTE_TOKEN = "'";

        public enum TokenTypes
        {
            Unknown,
            Whitespace,
            Newline,
            BlockCommentStart,
            BlockCommentBody,
            BlockCommentEnd,
            LineCommentStart,
            LineCommentBody,
            StringStart,
            StringBody,
            StringEnd,
            Star,
            Keyword,
            OpenParenthesis,
            CloseParenthesis,
            Comma,
            Semicolon,
            Period
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
        public int Length { get {
            if (TokenType == TokenTypes.StringBody)
            {
                return (Text.Replace("'","''")).Length;
            }
            return Text.Length;
        } }
        public string Text { get; set; }
        /// <summary>
        /// Indicates if the block comment start token or string start token isn't known to be closed such as /* or '
        /// </summary>
        public bool IsOpen { get; set; }

        
        
        public SqlToken(TokenTypes type, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            StartCharacterIndex = oneBasedStartCharacterIndex;
            LineNumber = oneBasedLineNumber;
            TokenType = type;
            IsOpen = false;
        }

        static public IList<SqlToken> ExtractTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex, SqlToken openTokenIfAny)
        {
            if (openTokenIfAny != null)
            {
                if (openTokenIfAny.TokenType == TokenTypes.BlockCommentStart)
                {
                    return ExtractBlockCommentTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex, openTokenIfAny);
                }
                else if (openTokenIfAny.TokenType == TokenTypes.StringStart)
                {
                    return ExtractStringTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex, openTokenIfAny);
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
            else if (isStringStart(charsToEvaluate))
            {
                return ExtractStringTokens(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
            }
            else if (isPunctuation(charsToEvaluate))
            {
                return ExtractPunctuationToken(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
            }
            else
            {
                string keyword = SqlKeyWords.GetSqlKeyWord(charsToEvaluate);
                if (keyword != "")
                {
                    return ExtractKeywordToken(oneBasedLineNumber, oneBasedStartCharacterIndex, keyword);
                }
            }
            return ExtractUnknownToken(charsToEvaluate, oneBasedLineNumber, oneBasedStartCharacterIndex);
        }

        private static IList<SqlToken> ExtractKeywordToken(int oneBasedLineNumber, int oneBasedStartCharacterIndex, string keyword)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            var t = new SqlToken(TokenTypes.Keyword, oneBasedLineNumber, oneBasedStartCharacterIndex);
            t.Text = keyword;
            tokens.Add(t);
            return tokens;
        }

        private static IList<SqlToken> ExtractUnknownToken(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();

            int unknownTokenEndIndex = -1;

            for (int charIndex = 0; charIndex < charsToEvaluate.Length; charIndex+=1)
            {
                if (isWhitespace(charsToEvaluate[charIndex]) ||
                    isLineCommentStart(charsToEvaluate, charIndex) ||
                    isBlockCommentStart(charsToEvaluate, charIndex) ||
                    isStringStart(charsToEvaluate,charIndex) ||
                    isPunctuation(charsToEvaluate,charIndex)) {
                        unknownTokenEndIndex = charIndex - 1;
                        break;
                }
            }

            var t = new SqlToken(TokenTypes.Unknown, oneBasedLineNumber, oneBasedStartCharacterIndex);
                
            if (unknownTokenEndIndex == -1)
            {
                t.Text = new String(charsToEvaluate);
            }
            else
            {
                t.Text = new String(charsToEvaluate, 0, unknownTokenEndIndex + 1);
            }
            tokens.Add(t);
            return tokens;

        }

        private static bool weAreAlreadyInAnOpenBlockComment(SqlToken openTokenIfAny = null)
        {
            return (openTokenIfAny != null && openTokenIfAny.TokenType == TokenTypes.BlockCommentStart && openTokenIfAny.IsOpen);
        }

        private static bool weAreAlreadyInAnOpenString(SqlToken openTokenIfAny = null)
        {
            return (openTokenIfAny != null && openTokenIfAny.TokenType == TokenTypes.StringStart && openTokenIfAny.IsOpen);
        }

        private static IList<SqlToken> ExtractBlockCommentTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex, SqlToken openTokenIfAny = null)
        {
            List<SqlToken> tokens = new List<SqlToken>();

            int offset = 0;
            SqlToken t;

            if (isBlockCommentStart(charsToEvaluate) && !weAreAlreadyInAnOpenBlockComment(openTokenIfAny))
            { 
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
                        t.Text = new String(charsToEvaluate, offset, i-offset);
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

        private static IList<SqlToken> ExtractStringTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex, SqlToken openTokenIfAny = null)
        {
            List<SqlToken> tokens = new List<SqlToken>();

            int offset = 0;
            SqlToken t;

            if (isStringStart(charsToEvaluate) && !weAreAlreadyInAnOpenString(openTokenIfAny))
            {
                t = new SqlToken(TokenTypes.StringStart, oneBasedLineNumber, oneBasedStartCharacterIndex);
                t.Text = QUOTE_TOKEN;
                offset += QUOTE_TOKEN.Length;
                tokens.Add(t);
            }

            if (thereAreStillCharactersRemaining(charsToEvaluate, offset))
            {
                StringBuilder remainingChars = new StringBuilder(charsToEvaluate.Length - offset);
                for (int i = offset; i < charsToEvaluate.Length; i += 1)
                {
                    if (isEscapedQuote(charsToEvaluate, i))
                    {
                        remainingChars.Append("'");
                        i += 1;
                    }
                    else if (isStringEnd(charsToEvaluate, i)) {
                        var body = remainingChars.ToString();
                        if (body.Length > 0) {
                            t = new SqlToken(TokenTypes.StringBody, oneBasedLineNumber, oneBasedStartCharacterIndex + offset);
                            t.Text = body;
                            tokens.Add(t);
                        }
                        t = new SqlToken(TokenTypes.StringEnd, oneBasedLineNumber, oneBasedStartCharacterIndex + i);
                        t.Text = QUOTE_TOKEN;
                        tokens.Add(t);
                        return tokens;
                    }
                    else
                    {
                        remainingChars.Append(charsToEvaluate[i]);
                    }
                }

                //We didn't find a string end (*/)

                //hack: fix this.
                foreach (var s in tokens.Where(tok => tok.TokenType == TokenTypes.StringStart))
                {
                    s.IsOpen = true;
                }

                t = new SqlToken(TokenTypes.StringBody, oneBasedLineNumber, oneBasedStartCharacterIndex + offset);
                t.Text = remainingChars.ToString();
                tokens.Add(t);
                return tokens;
            }

            //hack: fix this.
            foreach (var s in tokens.Where(tok => tok.TokenType == TokenTypes.StringStart))
            {
                s.IsOpen = true;
            }

            return tokens;
        }

        private static bool thereAreStillCharactersRemaining(Char[] charsToEvaluate, int offset)
        {
            return charsToEvaluate.Length > offset;
        }

        private static IList<SqlToken> ExtractLineCommentTokens(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            SqlToken t;
            
            t = new SqlToken(TokenTypes.LineCommentStart, oneBasedLineNumber, oneBasedStartCharacterIndex);
            t.Text = LINE_COMMENT_TOKEN;
            tokens.Add(t);

            if (thereAreStillCharactersRemaining(charsToEvaluate, LINE_COMMENT_TOKEN.Length))
            {
                t = new SqlToken(TokenTypes.LineCommentBody, oneBasedLineNumber, oneBasedStartCharacterIndex + LINE_COMMENT_TOKEN.Length);
                t.Text = new String(charsToEvaluate, 2, charsToEvaluate.Length - LINE_COMMENT_TOKEN.Length);
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

        private static IList<SqlToken> ExtractPunctuationToken(Char[] charsToEvaluate, int oneBasedLineNumber, int oneBasedStartCharacterIndex)
        {
            TokenTypes? tt = punctuationTokenType(charsToEvaluate[0]);
            if (!tt.HasValue) {
                throw new ArgumentException("Called Extract Punctuation Token without passing punctuation.");
            }

            List<SqlToken> tokens = new List<SqlToken>();
            SqlToken t = new SqlToken(tt.Value, oneBasedLineNumber, oneBasedStartCharacterIndex);
            t.Text = new String(charsToEvaluate,0,1);
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
        static private bool isLineCommentStart(Char[] theCharArray, int zeroBasedStartCharacterIndex = 0)
        {
            if (theCharArray.Length < zeroBasedStartCharacterIndex + 1)
            {
                return false;
            }
            return (theCharArray[0] == '-' && theCharArray[1] == '-');
        }
        static private bool isBlockCommentStart(Char[] theCharArray, int zeroBasedStartCharacterIndex = 0)
        {
            if (theCharArray.Length < zeroBasedStartCharacterIndex + 1)
            {
                return false;
            }
            return (theCharArray[0] == '/' && theCharArray[1] == '*');
        }
        static private bool isBlockCommentEnd(Char[] theCharArray, int firstCharIndex)
        {
            return (theCharArray[firstCharIndex] == '*' && theCharArray[firstCharIndex + 1] == '/');
        }
        static private TokenTypes? punctuationTokenType(Char character)
        {
            if (character == ',') { return TokenTypes.Comma; }
            if (character == '.') { return TokenTypes.Period; } //bug: will tokenize decimal numbers and floats...
            if (character == '(') { return TokenTypes.OpenParenthesis; }
            if (character == ')') { return TokenTypes.CloseParenthesis; }
            if (character == ';') { return TokenTypes.Semicolon; }
            return null;
        }
        static private bool isPunctuation(Char[] theCharArray, int firstCharIndex = 0)
        {
            return (punctuationTokenType(theCharArray[firstCharIndex]) != null);
        }
        static private bool isStringStart(Char[] theCharAray, int firstCharIndex = 0)
        {
            return (theCharAray[firstCharIndex] == '\'');
        }
        static private bool isEscapedQuote(Char[] theCharArray, int firstCharIndex)
        {
            if (firstCharIndex + 1 >= theCharArray.Length)
            {
                return false;
            }
            return (theCharArray[firstCharIndex] == '\'' && theCharArray[firstCharIndex + 1] == '\'') ;
        }
        static private bool isStringEnd(Char[] theCharAray, int firstCharIndex)
        {
            if (firstCharIndex == theCharAray.Length - 1)
            {
                return (theCharAray[firstCharIndex] == '\'');
            }
            return (theCharAray[firstCharIndex] == '\'' && theCharAray[firstCharIndex+1] != '\'');
        }


    }
}
