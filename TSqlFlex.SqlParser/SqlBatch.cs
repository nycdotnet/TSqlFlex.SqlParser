using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSqlFlex.SqlParser
{
    public class SqlBatch
    {
        private List<SqlToken> Tokens = new List<SqlToken>();
        private List<int> LineStartIndexes = new List<int>();
        private char[] BatchChars;

        private enum ParseStates
        {
            InAComment,
            InAString,
            Normal
        }

        private void parseSqlBatch()
        {
            // not fully implemented.
            int charIndex = 0;
            var parseState = ParseStates.Normal;
            while (charIndex < BatchChars.Length)
            {
                if (parseState == ParseStates.Normal)
                {
                    if (BatchChars[charIndex] == '-' && BatchChars[charIndex + 1] == '-')
                    {

                    }
                }
                charIndex += 1;
            }
        }

        public SqlBatch(string sqlBatchText)
        {
            BatchChars = sqlBatchText.ToCharArray();
            parseSqlBatch();
        }

        public SqlBatch(char[] sqlBatchChars)
        {
            BatchChars = sqlBatchChars;
            parseSqlBatch();
        }

        public string BatchText()
        {
            return new string(BatchChars);
        }


    }
}
