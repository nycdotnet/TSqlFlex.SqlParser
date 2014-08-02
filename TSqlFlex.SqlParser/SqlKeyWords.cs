using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser
{
    static public class SqlKeyWords
    {
        static private Dictionary<string, SqlToken.TokenTypes> SqlKeywords;
        static private int MaxKeywordLength;

        static SqlKeyWords()
        {
            InitializeSqlTokens();
        }

        static private void InitializeSqlTokens()
        {
            SqlKeywords = new Dictionary<string, SqlToken.TokenTypes>();
            SqlKeywords.Add("SELECT", SqlToken.TokenTypes.Select);
            SqlKeywords.Add("INSERT", SqlToken.TokenTypes.Insert);
            SqlKeywords.Add("UPDATE", SqlToken.TokenTypes.Update);
            SqlKeywords.Add("DELETE", SqlToken.TokenTypes.Delete);
            SqlKeywords.Add("FROM", SqlToken.TokenTypes.From);
            SqlKeywords.Add("*", SqlToken.TokenTypes.Star);

            MaxKeywordLength = SqlKeywords.Keys.Max(s => s.Length);
        }

        static public SqlToken.TokenTypes TokenTypeFromKeyWord(string keyWord)
        {
            return SqlKeywords[keyWord];
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
                    possibleKeyWords = SqlKeywords.Keys.Where(k => k[charIndex] == theCharAray[charIndex]).ToList<string>();
                }
                else
                {
                    //narrow down the list from the previous list
                    possibleKeyWords = possibleKeyWords.Where(s => s[charIndex] == theCharAray[charIndex]).ToList<string>();
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
