using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;
using Tracer.Poco;
using Tests.Suites.Slicing;
using Tests.Cases.Roslyn;
using Tests.Cases.Features;

namespace Tests
{
    [TestClass]
    public class ErrorSuite : BaseSlicingTest
    {
        //Motivo: Los resultados del slicer no son correctos.
        [TestMethod]
        public void GenericTypeConstraint()
        {
            var testedType = typeof(GenericTypeConstraint);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced = 
                {
                    sameFile.WithLine(13),
                    //sameFile.WithLine(15),
                    //sameFile.WithLine(23),
                    sameFile.WithLine(16),
                },
            }
            );
        }


        //No se esta teniendo en cuenta sobreescribir el ToString
        [TestMethod]
        public void OverrideToStringTest()
        {
            var testedType = typeof(OverrideToStringTest);
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
        //Motivo: Esta buscando this, cuando no deberia porque esta dentro de metodo estatico.
        [TestMethod]
        public void LambdaExpression()
        {
            var testedType = typeof(LambdaExpressions);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced = 
                {
                    sameFile.WithLine(15),
                    sameFile.WithLine(14),
                    sameFile.WithLine(16),
                },
            }
            );
        }

        //Motivo: No estamos instrumentando new[]
        [TestMethod]
        public void KeywordNewArray()
        {
            var testedType = typeof(KeywordVar);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(25),
                Sliced =
                {
                    sameFile.WithLine(18),
                    sameFile.WithLine(19),
                    sameFile.WithLine(23),
                    sameFile.WithLine(25),
                },
            });
        }

        //Motivo: No estamos instrumentado la keyword yield
        [TestMethod]
        public void KeywordYield()
        {
            var testedType = typeof(KeywordYield);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                //los assert estan puesto de ejemplo.
                //el problema esta cuando se consume la traza.
                //entones el slicer no soporta yield
                Criteria = sameFile.WithLine(16),
                Sliced = 
                {
                    sameFile.WithLine(14),
                    sameFile.WithLine(15),
                },
            }
            );
        }


        //Motivo: Los tipos nulos tiene metodos, esos no los estamos teniendo en cuenta.
        [TestMethod]
        public void NullableTypesWithMethods()
        {
            var testedType = typeof(NullableTypesWithMethods);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced = { 
                    sameFile.WithLine(13),
                    sameFile.WithLine(14),
                    sameFile.WithLine(16),
                },
            });
        }


        [TestMethod]
        public void GetConstructor()
        {
            var testedType = typeof(GetConstructor);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(26),
                        sameFile.WithLine(36),
                        sameFile.WithLine(34),
                        sameFile.WithLine(32),
                        sameFile.WithLine(33),
                        sameFile.WithLine(29),
                    },
            }
            );
        }

        [TestMethod]
        public void NewArrayInArgument()
        {
            var testedType = typeof(NewArrayInArgumenter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(17),
                        sameFile.WithLine(16),
                        sameFile.WithLine(28),
                        //sameFile.WithLine(24), // ==> El havoc benévolo evita que entre en el slice
                        sameFile.WithLine(23),
                        sameFile.WithLine(21),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        // No utiliza "a" directamente
                        //sameFile.WithLine(13),
                    },
            }
            );
        }

        [TestMethod]
        public void NotSupportedOutInCondition()
        {
            var testedType = typeof(NotSupportedOutInCondition);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(20),
                    },
            }
            );
        }

        [TestMethod]
        public void NotSupportedUsing()
        {
            var testedType = typeof(NotSupportedUsing);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(19),
                Sliced =
                    {
                        sameFile.WithLine(19),
                        sameFile.WithLine(30),
                    },
            }
            );
        }

        [TestMethod]
        public void NotSupportedUsingVarDeclaration()
        {
            var testedType = typeof(NotSupportedUsingVarDeclaration);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(19),
                Sliced =
                    {
                        sameFile.WithLine(19),
                        sameFile.WithLine(32),
                        sameFile.WithLine(17),
                        sameFile.WithLine(25),
                        sameFile.WithLine(23),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        //Motivo: Esta devolviendo mal el slice, la linea 14 no deberia devolverla. Esto es porque no se da cuenta
        // que ?? es un if
        [TestMethod]
        public void NullCoalescingOperator()
        {
            var testedType = typeof(NullCoalescingOperator);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced = 
                {
                    sameFile.WithLine(13),
                    sameFile.WithLine(16),
                    sameFile.WithLine(17),
                },
            }
            );
        }

        //Motivo: Devuelve mal el slice, esto es porque no entiende el condicional (?:)
        [TestMethod]
        public void ConditionalOperator()
        {
            var testedType = typeof(ConditionalOperator);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(17),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(17),
                    },
            }
            );
        }


        //Motivo: No se esta tratando el AnonymousObjectMemberDeclaratorSyntax
        [TestMethod]
        public void AnonymousTypes()
        {
            var testedType = typeof(AnonymousTypes);
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


        //Motivo: No se da cuenta que el c1 dentro del metodo es el “casa” pasado por parametro.
        [TestMethod]
        public void Overloading()
        {
            var testedType = typeof(Overloading);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(16),
                        sameFile.WithLine(25)
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynParenthesizedNullExpression()
        {
            var testedType = typeof(ParenthesizedNulleExpression);
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
        public void RoslynTestName()
        {
            var testedType = typeof(TestName);
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
        public void RoslynTernaryConditional()
        {
            var testedType = typeof(TernaryConditional);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(16)
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynTernaryConditionalWithNull()
        {
            var testedType = typeof(TernaryConditionalWithNull);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(15)
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynCallWithObjectRef()
        {
            var testedType = typeof(CallWithObjectRef);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(21),
                        sameFile.WithLine(26),
                        sameFile.WithLine(28),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynCallWithRef()
        {
            var testedType = typeof(CallWithRef);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        sameFile.WithLine(15),
                        sameFile.WithLine(22),
                        sameFile.WithLine(20),
                    },
            }
            );
        }

        [TestMethod]
        public void MemberAccessOfRefExpr()
        {
            var testedType = typeof(MemberAccessOfRefExpr);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(28),
                        sameFile.WithLine(30),
                        sameFile.WithLine(32),
                    },
            }
            );
        }

        [TestMethod]
        public void MemberAccessOfRefExprAsParameter()
        {
            var testedType = typeof(MemberAccessOfRefExprAsParameter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(16),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(15),
                        sameFile.WithLine(16),
                        sameFile.WithLine(21),
                        sameFile.WithLine(26),
                        sameFile.WithLine(28),
                    },
            }
            );
        }

        [TestMethod]
        public void RoslynCallWithOut()
        {
            var testedType = typeof(CallWithOut);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(21),
                    },
            }
            );
        }
        
        [TestMethod]
        public void RoslynCallWithOutNotInitialized()
        {
            var testedType = typeof(CallWithOutNotInitialized);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(21),
                    },
            }
            );
        }
        
        [TestMethod]
        public void RoslynCallWithOutAsParameter()
        {
            var testedType = typeof(CallWithOutAsParameter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(15),
                        sameFile.WithLine(26),
                    },
            }
            );
        }
        
        [TestMethod]
        public void RoslynIndex()
        {
            var testedType = typeof(index);
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
        public void ParamsBinaryExpression()
        {
            var testedType = typeof(ParamsBinaryExpression);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced =
                    {
                        sameFile.WithLine(13),
                        sameFile.WithLine(14),
                        //sameFile.WithLine(19),
                        //sameFile.WithLine(20),
                    },
            }
            );
        }

        [TestMethod]
        public void NewWithNoArgumentsAndInitializerCollection()
        {
            var testedType = typeof(NewWithNoArgumentsAndInitializersCollection);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(15),
                Sliced =
                    {
                        sameFile.WithLine(14),
                        sameFile.WithLine(13),
                        sameFile.WithLine(15),
                    },
            }
            );
        }

        [TestMethod]
        public void EmptyMain()
        {
            var testedType = typeof(EmptyMain);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(22),
                // Quizas se puede considerar un error que haga una linea de slice, pero
                // siempre arranca con una linea para poder hacer el slice, no parece tan
                // malo.
                Sliced =
                    {
                        sameFile.WithLine(11)
                    },
            }
            );
        }

        [TestMethod]
        public void StringInObjectParameter()
        {
            var testedType = typeof(StringInObjectParameter);
            var sameFile = SameFileStmtBuilder(testedType);
            TestSimpleSlice(testedType, new TestResult
            {
                Criteria = sameFile.WithLine(14),
                Sliced = {
                    sameFile.WithLine(14),
                },
            }
            );
        }

    }
}
