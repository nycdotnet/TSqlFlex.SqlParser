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
            SqlKeywords.Add("SELECT");
            SqlKeywords.Add("INSERT");
            SqlKeywords.Add("UPDATE");
            SqlKeywords.Add("DELETE");
            SqlKeywords.Add("FROM");
            SqlKeywords.Add("*");

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
                    possibleKeyWords = SqlKeywords.Where(k => k[charIndex] == theCharAray[charIndex]).ToList<string>();
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
