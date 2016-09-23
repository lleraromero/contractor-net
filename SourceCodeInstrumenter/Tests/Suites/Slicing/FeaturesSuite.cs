using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;
using Tracer.Poco;
using Tests.Cases.Roslyn;
using Tests.Cases.Features;

namespace Tests
{
    [TestClass]
    public class FeaturesSuite : BaseSlicingTest
    {
        //Generics
        [TestMethod]
        public void WithoutGenerics()
        {
            var testedType = typeof(WithoutGenerics);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced = 
                {
                    sameFile.WithLine(14),
                    sameFile.WithLine(16),
                    sameFile.WithLine(17),
                },
            }
            );
        }

        [TestMethod]
        public void WithGenerics()
        {
            var testedType = typeof(WithGenerics);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced = 
                {
                    sameFile.WithLine(14),
                    sameFile.WithLine(15),
                },
            }
            );
        }

       

        [TestMethod]
        public void KeywordVarSimple()
        {
            var testedType = typeof(KeywordVar);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(21),
                Sliced =
                {
                    sameFile.WithLine(15),
                    sameFile.WithLine(16),
                    sameFile.WithLine(21),
                },
            });
        }

        
    
        [TestMethod]
        public void NullableTypes()
        {
            var testedType = typeof(NullableTypes);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult 
            {
                Criteria = sameFile.WithLine(17),
                Sliced = { 
                    sameFile.WithLine(14),
                    sameFile.WithLine(15),
                    sameFile.WithLine(17),
                },
            });
        }

       

        [TestMethod]
        public void ObsoleteAttribute()
        {
            var testedType = typeof(ObsoleteAttributeTest);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(20),
                Sliced = { 
                    sameFile.WithLine(16),
                    sameFile.WithLine(20),
                },
            });
        }

        [TestMethod]
        public void DebuggerDisplayAttribute()
        {
            var testedType = typeof(DebuggerDisplayAttribute);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced = 
                {
                    sameFile.WithLine(16),
                    sameFile.WithLine(28),
                    sameFile.WithLine(26),
                },
            }
            );
        }

        [TestMethod]
        public void DebuggerBrowsableAttribute()
        {
            var testedType = typeof(DebuggerBrowsableAttribute);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced = 
                {
                    sameFile.WithLine(15),
                },
            }
            );
        }

        [TestMethod]
        public void FlagAttribute()
        {
            var testedType = typeof(FlagsAttributeTest);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(38),
                Sliced = 
                {
                    sameFile.WithLine(36),
                    sameFile.WithLine(38),
                },
            }
            );
        }

        [TestMethod]
        public void CondAttribute()
        {
            var testedType = typeof(CondAttribute);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(18),
                Sliced = 
                {
                    sameFile.WithLine(16),
                    sameFile.WithLine(18),
                },
            }
            );
        }

        



        [TestMethod]
        public void AutoProperties()
        {
            var testedType = typeof(AutoProperties);
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
        public void FormatString()
        {
            var testedType = typeof(FormatString);
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
        public void LiteralStrings()
        {
            var testedType = typeof(LiteralString);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(31),
                Sliced = 
                {
                    sameFile.WithLine(31),
                    sameFile.WithLine(30),
                    sameFile.WithLine(29),
                    sameFile.WithLine(21),
                    sameFile.WithLine(28),
                    sameFile.WithLine(20),
                    sameFile.WithLine(27),
                    sameFile.WithLine(19),
                    sameFile.WithLine(26),
                    sameFile.WithLine(18),
                    sameFile.WithLine(25),
                    sameFile.WithLine(17),
                    sameFile.WithLine(24),
                    sameFile.WithLine(16),
                    sameFile.WithLine(23),
                    sameFile.WithLine(15),
                    sameFile.WithLine(14),
                    sameFile.WithLine(13),
                },
            }
            );
        }

        [TestMethod]
        public void LambaExpressions()
        {
            var testedType = typeof(LambdaExpressionsWithoutDelegate);
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
        public void VariableNames()
        {
            var testedType = typeof(VariableNames);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced = 
                {
                    sameFile.WithLine(13),
                    sameFile.WithLine(14),
                },
            }
            );
        }

        [TestMethod]
        public void RoslynEmptyString()
        {
            var testedType = typeof(EmptyString);
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
        public void RoslynParenthesizedSimpleExpression()
        {
            var testedType = typeof(ParenthesizedSimpleExpression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynPrimaryExpressionsBool()
        {
            var testedType = typeof(PrimaryExpressionsTrue);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynStringLiteral()
        {
            var testedType = typeof(StringLiteral);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(31),
                Sliced =
                    {
                    sameFile.WithLine(31),
                    sameFile.WithLine(30),
                    sameFile.WithLine(29),
                    sameFile.WithLine(21),
                    sameFile.WithLine(28),
                    sameFile.WithLine(20),
                    sameFile.WithLine(27),
                    sameFile.WithLine(19),
                    sameFile.WithLine(26),
                    sameFile.WithLine(18),
                    sameFile.WithLine(25),
                    sameFile.WithLine(17),
                    sameFile.WithLine(24),
                    sameFile.WithLine(16),
                    sameFile.WithLine(23),
                    sameFile.WithLine(15),
                    sameFile.WithLine(14),
                    sameFile.WithLine(13),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynCharacterLiteralExpression()
        {
            var testedType = typeof(CharacterLiteralExpression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynNumericLiteralExpressionn()
        {
            var testedType = typeof(NumericLiteralExpression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynAssignmentOperators()
        {
            var testedType = typeof(AssignmentOperators);
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
        public void RoslynCall()
        {
            var testedType = typeof(Call);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(18),
                        sameFile.WithLine(16),
                        sameFile.WithLine(19),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynCast()
        {
            var testedType = typeof(Cast);
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
        public void RoslynCallWithNamedArgument()
        {
            var testedType = typeof(CallWithNamedArgument);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(23),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(22),
                        sameFile.WithLine(15),
                        sameFile.WithLine(20),
                        sameFile.WithLine(23),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynNew()
        {
            var testedType = typeof(TestNew);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        //sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(24),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynNewWithArgument()
        {
            var testedType = typeof(newWithArgument);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(22),
                        sameFile.WithLine(13),
                        sameFile.WithLine(20),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynNewWithNamedArgument()
        {
            var testedType = typeof(newWithNamedArgument);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(22),
                        sameFile.WithLine(13),
                        sameFile.WithLine(20),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynNewWithEmptyInitializer()
        {
            var testedType = typeof(NewWithEmptyInitializer);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynNewWithNoArgumentsAndEmptyInitializer()
        {
            var testedType = typeof(NewWithNoArgumentsAndEmptyInitializer);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        //sameFile.WithLine(13),
                        sameFile.WithLine(19),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynNewWithNoArgumentsAndInitializer()
        {
            var testedType = typeof(NewWithNoArgumentsAndInitializer);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                    },
            }
            );
        }


        [TestMethod]
        public void RoslynNewWithNoArgumentsAndInitializers()
        {
            var testedType = typeof(NewWithNoArgumentsAndInitializers);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(13),
                    },
            }
            );
        }


        [TestMethod]
        public void AssignOrder()
        {
            var testedType = typeof(AssignOrder);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(21),
                Sliced = 
                {
                    sameFile.WithLine(21)
                },
            }
            );
        }



    }
}