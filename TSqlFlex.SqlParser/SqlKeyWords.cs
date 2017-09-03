using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser
{
    static public class SqlKeyWords
    {
        static private SortedSet<string> SqlKeywords;
        static private int MaxKeywordLength;

        static SqlKeyWords()
        {
            InitializeSqlTokens();
        }

        static private void InitializeSqlTokens()
        {
            SqlKeywords = new SortedSet<string>();
            SqlKeywords.Add("*");
            SqlKeywords.Add("ACTION");
            SqlKeywords.Add("ALTER");
            SqlKeywords.Add("AS");
            SqlKeywords.Add("ASC");
            SqlKeywords.Add("BY");
            SqlKeywords.Add("CLUSTERED");
            SqlKeywords.Add("COLUMN");
            SqlKeywords.Add("CONSTRAINT");
            SqlKeywords.Add("CREATE");
            SqlKeywords.Add("DEFAULT");
            SqlKeywords.Add("DELETE");
            SqlKeywords.Add("DESC");
            SqlKeywords.Add("DROP");
            SqlKeywords.Add("EXEC");
            SqlKeywords.Add("EXECUTE");
            SqlKeywords.Add("FROM");
            SqlKeywords.Add("FUNCTION");
            SqlKeywords.Add("GROUP");
            SqlKeywords.Add("INSERT");
            SqlKeywords.Add("INTO");
            SqlKeywords.Add("INDEX");
            SqlKeywords.Add("KEY");
            SqlKeywords.Add("LEVEL");
            SqlKeywords.Add("NONCLUSTERED");
            SqlKeywords.Add("OCT");
            SqlKeywords.Add("ORDER");
            SqlKeywords.Add("PRIMARY");
            SqlKeywords.Add("PROC");
            SqlKeywords.Add("PROCEDURE");
            SqlKeywords.Add("RULE");
            SqlKeywords.Add("SCHEMA");
            SqlKeywords.Add("SELECT");
            SqlKeywords.Add("STATISTICS");
            SqlKeywords.Add("STATUS");
            SqlKeywords.Add("TABLE");
            SqlKeywords.Add("TRIGGER");
            SqlKeywords.Add("UPDATE");
            SqlKeywords.Add("USER");
            SqlKeywords.Add("VALUES");
            SqlKeywords.Add("VIEW");
            SqlKeywords.Add("WHERE");
            
            MaxKeywordLength = SqlKeywords.Max(s => s.Length);
        }

        //hack: This is probably an extraordinary candidate for optimization.
        static public string GetSqlKeyWord(Char[] theCharAray, int firstCharIndex = 0)
        {
            int charsToAnalyze = new int[] { theCharAray.Length, MaxKeywordLength }.Min();
            List<string> possibleKeyWords = null;

            for (int charIndex = 0; charIndex < charsToAnalyze; charIndex += 1)
            {
                var testChar = char.ToUpper(theCharAray[charIndex]);
                if (possibleKeyWords == null)
                {
                    possibleKeyWords = SqlKeywords.Where(k => k[charIndex] == theCharAray[charIndex]).ToList();
                }
                else
                {
                    //narrow down the list from the previous list
                    possibleKeyWords = possibleKeyWords.Where(s => s[charIndex] == theCharAray[charIndex]).ToList();
                }

                if (possibleKeyWords == null || possibleKeyWords.Count == 0)
                {
                    return "";
                }
                if (possibleKeyWords.Count == 1)
                {
                    //bug: need to not trigger keywords on incorrect tokens like "selecta"
                    string keyword = new String(theCharAray, 0, possibleKeyWords[0].Length).ToUpper();
                    if (possibleKeyWords[0] == keyword) {
                        return keyword;
                    }
                    else {
                        return "";
                    }
                }
            }
            return "";
        }

    }
}
