using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TSqlFlex.SqlParser;

namespace TSqlFlex.SqlParser.Tests
{
    [TestFixture()]
    public class ParserTests
    {

        [Test()]
        public void CreateParser_DoesNotCrash()
        {
            SqlBatch b = new SqlBatch("");
            Assert.IsNotNull(b, "should be able to create batch.");
        }

        [Test()]
        public void SqlBatch_ReturnsSameBatchTextAsPassed()
        {
            string s = StaticFiles.SimpleSelect();
            SqlBatch b = new SqlBatch(s);
            
            Assert.AreEqual(s, b.BatchText());
        }

        //todo: this is valid SQL: SELECT 1as[Z],'B'as[R[[z]
    }
}
