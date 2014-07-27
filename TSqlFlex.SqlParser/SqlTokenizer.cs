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
            do
            {
                string line = await sql.ReadLineAsync();
                if (line == null)
                {
                    return tokens;
                }

                lineNumber += 1;

                SqlToken currentToken = null;

                int charIndex = 0;
                while (charIndex < line.Length)
                {
                    if (currentToken == null)
                    {
                        currentToken = new SqlToken(line.Substring(charIndex, line.Length - charIndex).ToCharArray(), charIndex+1, lineNumber);
                    }

                    charIndex += currentToken.Length;
                    tokens.Add(currentToken);
                    currentToken = null;
                }
                if (!sql.EndOfStream)
                {
                    var newLineToken = new SqlToken(SqlToken.TokenTypes.Newline, line.Length+1, lineNumber);
                    newLineToken.Text = "\r\n";
                    tokens.Add(newLineToken);
                }

                Debug.Print(lineNumber.ToString() + ": " + line);
            } while (!sql.EndOfStream);

            return tokens;
        }
    }
}
