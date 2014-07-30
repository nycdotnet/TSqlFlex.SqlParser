using System;
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
        static public async Task<IList<SqlToken>> TokenizeAsync(string sql)
        {
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                sw.Write(sql);
                sw.Flush();
                ms.Position = 0;
                return await TokenizeAsync(ms, Encoding.UTF8);
            }
        }

        static public async Task<IList<SqlToken>> TokenizeAsync(Stream sqlStream, Encoding encoding)
        {
            List<SqlToken> tokens = new List<SqlToken>();
            StreamReader sql = new StreamReader(sqlStream, encoding);

            int lineNumber = 0;
            
            SqlToken openToken = null;
            int openTokenCount = 0;
            do
            {
                string line = await sql.ReadLineAsync();
                if (line == null)
                {
                    return tokens;
                }

                lineNumber += 1;
                
                int charIndex = 0;
                
                while (charIndex < line.Length)
                {
                    List<SqlToken> newTokens = SqlToken.ExtractTokens(line.Substring(charIndex, line.Length - charIndex).ToCharArray(), lineNumber, charIndex + 1, openToken).ToList<SqlToken>();
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

                Debug.Print(lineNumber.ToString() + ": " + line);
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
