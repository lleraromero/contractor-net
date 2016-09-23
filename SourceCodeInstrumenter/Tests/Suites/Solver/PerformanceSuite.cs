using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Cases.Solver;
using Tests.Util;

namespace Tests.Suites.Solver
{
    [TestClass]
    public class PerformanceSuite : BaseSlicingTest
    {
        [TestMethod]
        public void ChainNews()
        {
            var testedType = typeof(ChainNews);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(41),
                Sliced =
                    {
                         sameFile.WithLine(41)
                    },
            }
            );
        }
    }
}
