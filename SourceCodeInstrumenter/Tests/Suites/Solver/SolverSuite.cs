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
    public class SolverSuite : BaseSlicingTest
    {
        [TestMethod]
        public void SolverScalars()
        {
            var testedType = typeof(Scalars);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15)
                    },
            }
            );
        }

        [TestMethod]
        public void SolverInstrumentedNew()
        {
            var testedType = typeof(InstrumentedNew);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(13),
                Sliced =
                    {
                        sameFile.WithLine(13)
                    },
            }
            );
        }

        [TestMethod]
        public void SolverNonInstrumentedNew()
        {
            var testedType = typeof(NonInstrumentedNew);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(13),
                Sliced =
                    {
                        sameFile.WithLine(13)
                    },
            }
            );
        }

        [TestMethod]
        public void SolverNonInstrumentedNewWithArguments()
        {
            var testedType = typeof(NonInstrumentedNewWithArguments);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        [TestMethod]
        public void SolverNonInstrumentedChainObjects()
        {
            var testedType = typeof(NonInstrumentedChainObjects);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        [TestMethod]
        public void SolverNonInstrumentedInvocation()
        {
            var testedType = typeof(NonInstrumentedInvocation);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        [TestMethod]
        public void SolverCallback0()
        {
            var testedType = typeof(SolverCallback0);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                    },
            }
            );
        }

        [TestMethod]
        public void SolverPtgVertexDisposing()
        {
            var testedType = typeof(PtgVertexDisposing);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                    },
            }
            );
        }

        [TestMethod]
        public void SolverHubCompression()
        {
            var testedType = typeof(HubCompression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(24),
                Sliced =
                    {
                        sameFile.WithLine(24),
                    },
            }
            );
        }

        [TestMethod]
        public void ListAddWithReadOnlyLambda()
        {
            var testedType = typeof(ListAddWithReadOnlyLambda);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(17),
                        sameFile.WithLine(16),
                        sameFile.WithLine(15),
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                    },
            }
            );
        }
    }
}
