using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;
using Tracer.Poco;

namespace Tests
{
    [TestClass]
    public class CallbackSuite : BaseSlicingTest
    {
        [TestMethod]
        public void Callback2different()
        {
            var testedType = typeof(Callback2different);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(42),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(22),
                        sameFile.WithLine(23),
                        sameFile.WithLine(27),
                        sameFile.WithLine(30),
                        sameFile.WithLine(33),
                        sameFile.WithLine(39),
                        sameFile.WithLine(40),
                        sameFile.WithLine(41),
                        sameFile.WithLine(42),
                        sameFile.WithLine(47),
                        sameFile.WithLine(48),
                        sameFile.WithLine(65),
                        sameFile.WithLine(68),
                        sameFile.WithLine(72),
                        sameFile.WithLine(73),

                        // TODO: USO DEL THIS
                        //sameFile.WithLine(24),
                        //sameFile.WithLine(36),
                        //sameFile.WithLine(45),

                        // DEP DE CONTROL X CALLBACK
                        sameFile.WithLine(49), // IF
                        //sameFile.WithLine(50),
                        //sameFile.WithLine(52),
                    },
            }
            );
        }

        [TestMethod]
        public void Callback0()
        {
            var testedType = typeof (Callback0);
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
        public void Callback0FollowLdOutside()
        {
            var testedType = typeof (Callback0FollowLdOutside);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(18),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        sameFile.WithLine(18),
                        //sameFile.WithLine(25), TODO: Preguntar uso del this
                        sameFile.WithLine(27),
                    },
            }
            );
        }

        [TestMethod]
        public void Callback1()
        {
            var testedType = typeof (Callback1);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        //sameFile.WithLine(23), //TODO: Uso del this
                        sameFile.WithLine(25),
                        sameFile.WithLine(26),
                    },
            }
            );
        }
        [TestMethod]
        public void Callback1R()
        {
            var testedType = typeof (Callback1R);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        sameFile.WithLine(24),
                        //aca agregué la 26, no entiendo cómo resultado no va a depender
                        //del valor de retorno si se está asignando???
                        sameFile.WithLine(26),
                    },
            }
            );
        }

        [TestMethod]
        public void Callback1RInstrumentedOutside()
        {
            var testedType = typeof (Callback1RInstrumentedOutside);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        sameFile.WithLine(21),
                        sameFile.WithLine(23),
                        sameFile.WithLine(29),
                        //aca agregué la 31, no entiendo cómo resultado no va a depender
                        //del valor de retorno si se está asignando???
                        sameFile.WithLine(31),
                    },
            }
            );
        }

        [TestMethod]
        public void Callback1RNonInstrumentedOutside()
        {
            var testedType = typeof (Callback1RNonInstrumentedOutside);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        sameFile.WithLine(24),
                        //aca agregué la 26, no entiendo cómo resultado no va a depender
                        //del valor de retorno si se está asignando???
                        sameFile.WithLine(26),
                    },
            }
            );
        }

        [TestMethod]
        public void Callback0Twice()
        {
            var testedType = typeof (Callback0Twice);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(17),
                    },
            }
            );
        }

    }
}
