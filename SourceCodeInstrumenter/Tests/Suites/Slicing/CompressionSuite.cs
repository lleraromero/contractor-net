using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;

namespace Tests.Suites.Slicing
{
    [TestClass]
    public class CompressionSuite : BaseSlicingTest
    {
        [TestMethod]
        public void Figure7()
        {
            var testedType = typeof (Figure7);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(32),
                Sliced =
                    {
                        sameFile.WithLine(32),
                        sameFile.WithLine(30),
                        sameFile.WithLine(29),
                        sameFile.WithLine(23),
                        sameFile.WithLine(21),
                        sameFile.WithLine(20),
                        sameFile.WithLine(18),
                        sameFile.WithLine(17),
                        sameFile.WithLine(16),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        [TestMethod]
        public void WhileMultiple()
        {
            var testedType = typeof(WhileMultiple);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(20),
                Sliced =
                    {
                        sameFile.WithLine(20),
                        sameFile.WithLine(18),
                        sameFile.WithLine(16),
                        sameFile.WithLine(15),
                        sameFile.WithLine(13),
                    },
            }
            );
        }
    }
}
