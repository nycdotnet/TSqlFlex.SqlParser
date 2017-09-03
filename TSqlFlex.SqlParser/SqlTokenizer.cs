﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser
{
    public class SqlTokenizer
    {
        /// <summary>
        /// Tokenizes the SQL statements in the stream
        /// </summary>
        /// <param name="sql">SQL statements to parse.</param>
        /// <returns>The IList of tokens in the SQL stream</returns>
        static public async Task<IList<SqlToken>> TokenizeAsync(string sql)
        {
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms, Encoding.UTF8);
                sw.Write(sql);
                sw.Flush();
                ms.Position = 0;
                return await TokenizeAsync(ms, Encoding.UTF8);
            }
        }

        /// <summary>
        /// Tokenizes the SQL statements in the stream
        /// </summary>
        /// <param name="sqlStream">A stream of the SQL statements to parse.</param>
        /// <param name="encoding">System.Text.Encoding of the stream.  If not specified, defaults to UTF8.</param>
        /// <returns>The IList of tokens in the SQL stream</returns>
        static public async Task<IList<SqlToken>> TokenizeAsync(Stream sqlStream, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            List<SqlToken> tokens = new List<SqlToken>();
            StreamReader sql = new StreamReader(sqlStream, encoding);

            int lineNumber = 0;
            
            SqlToken openToken = null;
            int openTokenCount = 0;
            do
            {
                var line = await sql.ReadLineAsync();
                if (line == null)
                {
                    return tokens;
                }

                lineNumber += 1;
                
                int charIndex = 0;
                
                while (charIndex < line.Length)
                {
                    List<SqlToken> newTokens = SqlToken.ExtractTokens(line.Substring(charIndex, line.Length - charIndex).ToCharArray(), lineNumber, charIndex + 1, openToken).ToList();
                    charIndex += LengthOfTokens(newTokens);
                    
                    foreach(SqlToken t in newTokens)
                    {
                        if (t.IsOpen)
                        {
                            openTokenCount += 1;
                            Debug.Assert(openTokenCount < 2, "There should only ever be 0 or 1 open tokens.");
                            openToken = t;
                        }
                    }

                    if (openTokenCount == 0)
                    {
                        openToken = null;
                    }
                    else
                    {
                        foreach (var t in newTokens)
                        {
                            if (t.TokenType == SqlToken.TokenTypes.BlockCommentEnd && openToken.TokenType == SqlToken.TokenTypes.BlockCommentStart)
                            {
                                openTokenCount -= 1;
                                openToken.IsOpen = false;
                                openToken = null;
                            }
                            else if (t.TokenType == SqlToken.TokenTypes.StringEnd && openToken.TokenType == SqlToken.TokenTypes.StringStart)
                            {
                                openTokenCount -= 1;
                                openToken.IsOpen = false;
                                openToken = null;
                            }
                        }
                    }
                    tokens.AddRange(newTokens);
                }
                if (!sql.EndOfStream)
                {
                    var newLineToken = new SqlToken(SqlToken.TokenTypes.Newline, lineNumber, line.Length + 1);
                    newLineToken.Text = "\r\n";
                    tokens.Add(newLineToken);
                }
            } while (!sql.EndOfStream);

            return tokens;
        }

        static private int LengthOfTokens(IEnumerable<SqlToken> tokens)
        {
            int total = 0;
            foreach (var item in tokens)
            {
                total += item.Length;
            }
            return total;
        }
    }
}
