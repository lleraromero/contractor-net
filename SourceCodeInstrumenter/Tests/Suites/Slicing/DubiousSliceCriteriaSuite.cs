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
    public class DubiousSliceCriteriaSuite : BaseSlicingTest
    {
        [TestMethod]
        public void InvocationChainWithCall()
        {
            var testedType = typeof(InvocationChainWithCall);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(25),
                        sameFile.WithLine(35),
                        sameFile.WithLine(43),
                        sameFile.WithLine(45),
                        sameFile.WithLine(54),
                    },
            }
            );
        }

        [TestMethod]
        public void InvocationChainWithProperty()
        {
            var testedType = typeof(InvocationChainWithProperty);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(26),
                        sameFile.WithLine(41),
                        sameFile.WithLine(43),
                        sameFile.WithLine(52),
                    },
            }
            );
        }

        [TestMethod]
        public void PreProcessorDirectiveInstrumentation()
        {
            var testedType = typeof(PreprocessorDirectiveInstrumentation);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(25),
                Sliced =
                    {
                        sameFile.WithLine(22),
                        sameFile.WithLine(25),
                    },
            }
            );
        }

        [TestMethod]
        public void SimpleNonInstrumentedString()
        {
            var testedType = typeof (SimpleNonInstrumentedString);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(19),
                Sliced =
                    {
                        //aca agregué la 23, no entiendo cómo valor no va a depender
                        //del get de Obj si está accediendo a Obj???
                        //sameFile.WithLine(23),
                        sameFile.WithLine(19),
                        sameFile.WithLine(18),
                        sameFile.WithLine(17),
                        sameFile.WithLine(16),
                        sameFile.WithLine(15),
                        sameFile.WithLine(14),
                    },
            }
            );
        }

        [TestMethod]
        public void SimpleNonInstrumentedObject()
        {
            var testedType = typeof (SimpleNonInstrumentedObject);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(19),
                Sliced =
                    {
                        //aca agregué la 23, no entiendo cómo valor no va a depender
                        //del get de Obj si está accediendo a Obj???
                        //sameFile.WithLine(23),
                        sameFile.WithLine(19),
                        sameFile.WithLine(18),
                        sameFile.WithLine(17),
                        sameFile.WithLine(16),
                        sameFile.WithLine(15),
                        sameFile.WithLine(14),
                    },
            }
            );
        }
    }
}