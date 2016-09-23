using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;
using Tracer.Poco;

namespace Tests
{
    [TestClass]
    public class OnlyScalarsSuite : BaseSlicingTest
    {
        [TestMethod]
        public void DynamicSlicingPaperExample1XIquals0()
        {
            var testedType = typeof (DynamicSlicingPaperExample1XIquals0);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(29),
                Sliced =
                    {
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(17),
                        sameFile.WithLine(19),
                        sameFile.WithLine(29),
                    },
            }
            );

            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(31),
                Sliced =
                    {
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(17),
                        sameFile.WithLine(20),
                        sameFile.WithLine(31),
                    },
            }
            );
        }

        [TestMethod]
        public void DynamicSlicingPaperExample1XLess0()
        {
            var testedType = typeof (DynamicSlicingPaperExample1XLess0);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(35),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(18),
                        sameFile.WithLine(35),
                    },
            }
            );

            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(37),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(19),
                        sameFile.WithLine(37),
                    },
            }
            );
        }

        [TestMethod]
        public void DynamicSlicingPaperExample2NIquals2()
        {
            var testedType = typeof (DynamicSlicingPaperExample2NIquals2);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(23),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(17),
                        sameFile.WithLine(19),
                        sameFile.WithLine(20),
                        sameFile.WithLine(21),
                        sameFile.WithLine(23),
                    },
            }
            );
        }

        //[TestMethod]
        //public void LegatingByControlStmt()
        //{
        //    var testedType = typeof (LegatingByControlStmt);
        //    var sameFile = SameFileStmtBuilder(testedType);
        //    TestSimpleSlice(testedType, new TestResult
        //    {
        //        Criteria = sameFile.WithLine(19),
        //        Sliced =
        //            {
        //                sameFile.WithLine(19),
        //            },
        //    }
        //    );

        //    TestSimpleSlice(testedType, new TestResult
        //    {
        //        Criteria = sameFile.WithLine(26),
        //        Sliced =
        //            {
        //                sameFile.WithLine(13),
        //                sameFile.WithLine(15),
        //                sameFile.WithLine(17),
        //                sameFile.WithLine(19),
        //                sameFile.WithLine(20),
        //                sameFile.WithLine(22),
        //                sameFile.WithLine(26),
        //            },
        //    }
        //    );
        //}

        [TestMethod]
        public void VerySimpleSlicing()
        {
            var testedType = typeof (VerySimpleSlicing);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(29),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(18),
                        sameFile.WithLine(29),
                    },
            }
            );

            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(32),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(21),
                        sameFile.WithLine(22),
                        sameFile.WithLine(24),
                        sameFile.WithLine(25),
                        sameFile.WithLine(32),
                    },
            }
            );

            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(35),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(18),
                        sameFile.WithLine(21),
                        sameFile.WithLine(22),
                        sameFile.WithLine(24),
                        sameFile.WithLine(25),
                        sameFile.WithLine(29),
                        sameFile.WithLine(32),
                        sameFile.WithLine(35),
                    },
            }
            );
        }

        [TestMethod]
        public void SimpleCallIgnoreParameter()
        {
            Type testedType = typeof(SimpleCallIgnoreParameter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(10),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(14),
                        sameFile.WithLine(16),
                    },
            }
            );
        }
        
        [TestMethod]
        public void SlicingSimpleCallFirstStmtIf()
        {
            Type testedType = typeof(SlicingSimpleCallFirstStmtIf);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(26),
                    },
            }
            );
        }

        [TestMethod]
        public void ScalarDetectCallInCondition()
        {
            Type testedType = typeof(ScalarDetectCallInCondition);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(22),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(16),
                        sameFile.WithLine(22),
                        sameFile.WithLine(25),
                        sameFile.WithLine(27),
                        sameFile.WithLine(29),
                    },
            }
            );
        }

        [TestMethod]
        public void SimpleCallNestedCall()
        {
            Type testedType = typeof(SimpleCallNestedCall);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(8),
                        sameFile.WithLine(10),
                        sameFile.WithLine(11),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(20),
                        sameFile.WithLine(22),
                        sameFile.WithLine(25),
                        sameFile.WithLine(27),
                        sameFile.WithLine(30),
                        sameFile.WithLine(32),
                    },
            }
            );
        }

        //[TestMethod]
        public void SimpleCallNestedConstructor()
        {
            Type testedType = typeof(SimpleCallNestedConstructor);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                        sameFile.WithLine(20),
                        sameFile.WithLine(21),
                        sameFile.WithLine(30),
                    },
            }
            );
        }


        [TestMethod]
        public void SimpleCallNestedExpression()
        {
            Type testedType = typeof(SimpleCallNestedExpression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(8),
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(11),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(20),
                        sameFile.WithLine(22),
                        sameFile.WithLine(25),
                        sameFile.WithLine(27),
                        sameFile.WithLine(30),
                        sameFile.WithLine(32),
                    },
            }
            );
        }

        [TestMethod]
        public void SimpleCallNestedExpressionOutside()
        {
            Type testedType = typeof(SimpleCallNestedExpressionOutside);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(8),
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(11),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(20),
                        sameFile.WithLine(22),
                        sameFile.WithLine(25),
                        sameFile.WithLine(27),
                        sameFile.WithLine(30),
                        sameFile.WithLine(32),
                    },
            }
            );
        }

        [TestMethod]
        public void CallWithinAnotherCall()
        {
            Type testedType = typeof(CallWithinAnotherCall);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(8),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(8),
                        sameFile.WithLine(11),
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(17),
                        sameFile.WithLine(19),
                    },
            }
            );
        }


        /// <summary>
        /// FIXME: works but `j` shouldn't be connected to `(inicial == 1)` by control. Related to early return (bug #10)
        /// </summary>
        [TestMethod]
        public void CallRecursive()
        {
            Type testedType = typeof(CallRecursive);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(9),
                Sliced =
                    {
                        sameFile.WithLine(7),
                        sameFile.WithLine(8),
                        sameFile.WithLine(9),
                        sameFile.WithLine(12),
                        sameFile.WithLine(14),
                        sameFile.WithLine(16),
                    },
            }
            );
        }

        [TestMethod]
        public void NonInstrumentedCall()
        {
            Type testedType = typeof(NonInstrumentedCall);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(8),
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(11),
                    },
            }
            );
        }

        [TestMethod]
        public void NonInstrumentedCallAsParameter()
        {
            Type testedType = typeof(NonInstrumentedCallAsParameter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(11),
                Sliced =
                    {
                        sameFile.WithLine(8),
                        sameFile.WithLine(9),
                        sameFile.WithLine(10),
                        sameFile.WithLine(11),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(19),
                        sameFile.WithLine(21),
                        sameFile.WithLine(23),
                        sameFile.WithLine(25),
                    },
            }
            );
        }


        [TestMethod]
        public void ImplicitConstructor()
        {
            Type testedType = typeof(ImplicitConstructor);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(12),
                Sliced =
                    {
                        sameFile.WithLine(8),
                        sameFile.WithLine(12),
                    },
            }
            );
        }

        [TestMethod]
        public void ExplicitConstructorPrefixedThis()
        {
            Type testedType = typeof(ExplicitConstructorPrefixedThis);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(23),
                        sameFile.WithLine(24),
                    },
            }
            );
        }

        [TestMethod]
        public void ExplicitConstructorUnprefixedThis()
        {
            Type testedType = typeof(ExplicitConstructorUnprefixedThis);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(23),
                        sameFile.WithLine(24),
                    },
            }
            );
        }

        [TestMethod]
        public void ScalarSimpleFor()
        {
            Type testedType = typeof(ScalarSimpleFor);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(19),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                        sameFile.WithLine(19),
                    },
            }
            );
        }

        [TestMethod]
        public void ScalarForWithDeclaration()
        {
            Type testedType = typeof(ScalarForWithDeclaration);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(18),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(16),
                        sameFile.WithLine(18),
                    },
            }
            );
        }

        [TestMethod]
        public void BreakingGlobal()
        {
            Type testedType = typeof(BreakingGlobal);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(23),
                Sliced =
                    {
                        sameFile.WithLine(11),
                        sameFile.WithLine(18),
                        sameFile.WithLine(19),
                        sameFile.WithLine(21),
                        sameFile.WithLine(23),
                    },
            }
            );
        }

        [TestMethod]
        public void ScalarDoubleUse()
        {
            Type testedType = typeof(ScalarDoubleUse);
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
        public void ScalarNonSettedField()
        {
            Type testedType = typeof(ScalarNonSettedField);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(19),
                    },
            }
            );
        }

        [TestMethod]
        public void ClassConstrWithoutInstructions()
        {
            Type testedType = typeof(ClassConstrWithoutInstructions);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(13),
                Sliced =
                    {
                        sameFile.WithLine(13),
                    },
            }
            );
        }

        [TestMethod]
        public void ConstField()
        {
            Type testedType = typeof(ConstField);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(15),
                        sameFile.WithLine(20),
                    },
            }
            );
        }
    }
}