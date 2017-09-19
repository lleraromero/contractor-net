using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Cases;
using Tests.Util;

namespace Tests.Suites.Slicing
{
    [TestClass]
    public class NotSupportedSuite : BaseSlicingTest
    {
        

        [TestMethod]
        public void NotSupportedForeachWithCall()
        {
            var testedType = typeof(NotSupportedForeachWithCall);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(21),
                Sliced =
                    {
                        sameFile.WithLine(21),
                        sameFile.WithLine(19),
                        sameFile.WithLine(16),
                        sameFile.WithLine(27),
                        //sameFile.WithLine(25), // USO DEL THIS. TODO: Para mi no va.
                        sameFile.WithLine(32),
                        sameFile.WithLine(30),
                        sameFile.WithLine(17),
                        sameFile.WithLine(15),
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                    },
            }
            );
        }       
    }
}
