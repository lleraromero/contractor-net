using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Cases;
using Tests.Util;
using Tracer.Poco;

namespace Tests
{
	[TestClass]
	public class GeneralSuite : BaseSlicingTest
	{
		[TestMethod]
		public void ListAddingElements()
		{
			var testedType = typeof(ListAddingElements);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(17),
				Sliced =
					{
						sameFile.WithLine(17),
						//estas 3 lineas no se slicean porque list.Add no modifica el puntero sino que solamente el objeto interno (en el aPtG)
						//sameFile.WithLine(16),
						//sameFile.WithLine(15),
						//sameFile.WithLine(14),
						sameFile.WithLine(13),
					},
			}
			);
		}
		[TestMethod]
		public void ListAddingElementsCount()
		{
			var testedType = typeof(ListAddingElementsCount);
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
		[TestMethod]
		public void MethodFromTwoReceptors()
		{
			var testedType = typeof(MethodFromTwoReceptors);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(22),
				Sliced =
					{
						sameFile.WithLine(22),
					},
			}
			);
		}
		[TestMethod]
		public void MethodFromTwoReceptorsWithDependencies()
		{
			var testedType = typeof(MethodFromTwoReceptorsWithDependencies);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(20),
				Sliced =
					{
						sameFile.WithLine(20),
						sameFile.WithLine(36),
						sameFile.WithLine(35),
						sameFile.WithLine(33),
						sameFile.WithLine(28),
						sameFile.WithLine(26),
						sameFile.WithLine(15),
					},
			}
			);
		}
		[TestMethod]
		public void MethodFromTwoReceptorsWithDoubleDependencies()
		{
			var testedType = typeof(MethodFromTwoReceptorsWithDoubleDependencies);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(20),
				Sliced =
					{
						sameFile.WithLine(20),
						sameFile.WithLine(36),
						sameFile.WithLine(35),
						sameFile.WithLine(33),
						sameFile.WithLine(28),
						sameFile.WithLine(26),
						sameFile.WithLine(15),
						sameFile.WithLine(18),
						sameFile.WithLine(17),
						sameFile.WithLine(14),
					},
			}
			);
		}
		[TestMethod]
		public void AliasingTestCopyRef()
		{
			var testedType = typeof (AliasingTestCopyRef);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(10),
						sameFile.WithLine(11),
						sameFile.WithLine(12),
						sameFile.WithLine(13),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingTestCopyScalar()
		{
			var testedType = typeof(AliasingTestCopyScalar);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(11),
				Sliced =
					{
						sameFile.WithLine(10),
						sameFile.WithLine(11),
					},
			}
			);
		}

		[TestMethod]
		public void CallWithReferencesParameters()
		{
			var testedType = typeof(CallWithReferencesParameters);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(21),
				Sliced =
					{
						sameFile.WithLine(17),
						sameFile.WithLine(19),
						sameFile.WithLine(20),
						sameFile.WithLine(21),
						sameFile.WithLine(24),
						sameFile.WithLine(26),
						sameFile.WithLine(27),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingReturningReference()
		{
			var testedType = typeof(AliasingReturningReference);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(9),
				Sliced =
					{
						sameFile.WithLine(8),
						sameFile.WithLine(9),
						sameFile.WithLine(14),
						sameFile.WithLine(17),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingUsingSingleThis()
		{
			var testedType = typeof(AliasingUsingSingleThis);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(10),
						sameFile.WithLine(12),
						sameFile.WithLine(13),
						sameFile.WithLine(21),
						sameFile.WithLine(23),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingUsingNestedThis()
		{
			var testedType = typeof(AliasingUsingNestedThis);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(12),
				Sliced =
					{
						sameFile.WithLine(8),
						sameFile.WithLine(11),
						sameFile.WithLine(12),
						sameFile.WithLine(21),
						sameFile.WithLine(22),
						sameFile.WithLine(23),
						sameFile.WithLine(24),
						sameFile.WithLine(29),
						sameFile.WithLine(38),
						sameFile.WithLine(40),
						sameFile.WithLine(41),
						sameFile.WithLine(46),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingUsingNestedThis2nd()
		{
			var testedType = typeof(AliasingUsingNestedThis);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(40),
				Sliced =
					{
						sameFile.WithLine(40),
						sameFile.WithLine(46),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingPassingReference()
		{
			var testedType = typeof(AliasingPassingReference);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(10),
				Sliced =
					{
						sameFile.WithLine(10),
						sameFile.WithLine(20),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingAssigningReference()
		{
			var testedType = typeof(AliasingAssigningReference);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(12),
				Sliced =
					{
						sameFile.WithLine(12),
						sameFile.WithLine(21),
						sameFile.WithLine(33),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingGlobal()
		{
			var testedType = typeof(AliasingGlobal);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(9),
						sameFile.WithLine(10),
						sameFile.WithLine(11),
					},
			}
				);
		}
		[TestMethod]
		public void AliasingGlobalPrefix()
		{
			var testedType = typeof(AliasingGlobalPrefix);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(9),
						sameFile.WithLine(10),
						sameFile.WithLine(11),
					},
			}
				);
		}

		[TestMethod]
		public void AliasingGlobalWithConstructor()
		{
			var testedType = typeof(AliasingGlobalWithConstructor);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(16),
				Sliced =
					{
						sameFile.WithLine(16),
						sameFile.WithLine(15),
						sameFile.WithLine(14),
					},
			}
				);
		}


		//[TestMethod]
		//public void AliasingSwitchCaseWithoutBreak()
		//{
		//    var testedType = typeof(AliasingSwitchCaseWithoutBreak);
		//    var sameFile = SameFileStmtBuilder(testedType);
		//    TestSimpleSlice(testedType, new TestResult
		//    {
		//        Criteria = sameFile.WithLine(16),
		//        Sliced =
		//            {
		//                sameFile.WithLine(7),
		//                sameFile.WithLine(8),
		//                sameFile.WithLine(9),
		//                sameFile.WithLine(16),
		//            },
		//    }
		//        );
		//}

		[TestMethod]
		public void NonInstrumentedCallParentInMetadata()
		{
			var testedType = typeof(NonInstrumentedCallParentInMetadata);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(11),
				Sliced =
					{
						sameFile.WithLine(9),
						sameFile.WithLine(10),
						sameFile.WithLine(11),
					},
			}
				);
		}

		[TestMethod]
		public void NonInstrumentedStatic()
		{
			var testedType = typeof(NonInstrumentedStatic);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(11),
				Sliced =
					{
						sameFile.WithLine(9),
						sameFile.WithLine(10),
						sameFile.WithLine(11),
					},
			}
				);
		}

		[TestMethod]
		public void SwitchCase()
		{
			var testedType = typeof(SwitchCase);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(22),
				Sliced =
					{
						sameFile.WithLine(7),
						sameFile.WithLine(9),
						sameFile.WithLine(16),
						sameFile.WithLine(22),
					},
			}
				);
		}

		[TestMethod]
		public void AliasingDispatch()
		{
			var testedType = typeof(AliasingDispatch);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(9),
				Sliced =
					{
						sameFile.WithLine(9),
						//Alejandro cree que estas 3 líneas también debieran formar parte del Slice
						//seguramente quedarán automáticamente cuando solucionemos el tema de
						//las dependencias de control por llamada
						//sameFile.WithLine(8),
						//sameFile.WithLine(14),
						//sameFile.WithLine(16),
					},
			}
				);
		}

		[TestMethod]
		public void BreakingWhileInsideIf()
		{
			var testedType = typeof(BreakingWhileInsideIf);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(7),
						sameFile.WithLine(9),
						sameFile.WithLine(11),
						sameFile.WithLine(15),
					},
			}
				);
		}

		[TestMethod]
		public void SlicingDispatchLookupNonInstrumented()
		{
			var testedType = typeof(SlicingDispatchLookupNonInstrumented);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(8),
				Sliced =
					{
						sameFile.WithLine(7),
						sameFile.WithLine(8),
					},
			}
				);
		}

		[TestMethod]
		public void SlicingDispatchLookupInstrumented()
		{
			var testedType = typeof(SlicingDispatchLookupInstrumented);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(8),
				Sliced =
					{
						sameFile.WithLine(8),
						sameFile.WithLine(15),
					},
			}
				);
		}
		//[TestMethod]
		//public void AliasingBankTransferSample()
		//{
		//    var testedType = typeof(AliasingBankTransferSample);
		//    var sameFile = SameFileStmtBuilder(testedType);
		//    TestSimpleSlice(testedType, new TestResult
		//    {
		//        Criteria = sameFile.WithLine(21),
		//        Sliced =
		//            {
		//                sameFile.WithLine(9),
		//                sameFile.WithLine(11),
		//                sameFile.WithLine(12),
		//                sameFile.WithLine(21),
		//                sameFile.WithLine(32),
		//            },
		//    }
		//    );
		//}

		//[TestMethod]
		//public void CallConstructor()
		//{
		//    var testedType = typeof(CallConstructor);
		//    var sameFile = SameFileStmtBuilder(testedType);
		//    TestSimpleSlice(testedType, new TestResult
		//    {
		//        Criteria = sameFile.WithLine(12),
		//        Sliced =
		//            {
		//                sameFile.WithLine(7),
		//                sameFile.WithLine(10),
		//                sameFile.WithLine(11),
		//                sameFile.WithLine(12),
		//                sameFile.WithLine(20),
		//            },
		//    }
		//    );
		//}
		//[TestMethod]
		//public void TestFieldInit()
		//{
		//    var testedType = typeof(TestFieldInit);
		//    var sameFile = SameFileStmtBuilder(testedType);
		//    TestSimpleSlice(testedType, new TestResult
		//    {
		//        Criteria = sameFile.WithLine(12),
		//        Sliced =
		//            {
		//                sameFile.WithLine(7),
		//                sameFile.WithLine(10),
		//                sameFile.WithLine(11),
		//                sameFile.WithLine(12),
		//                sameFile.WithLine(20),
		//            },
		//    }
		//    );
		//}

		[TestMethod]
		public void AliasingAssigningSimpleRef()
		{
			var testedType = typeof (AliasingAssigningSimpleRef);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(16),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(16),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingAllocOnNull()
		{
			var testedType = typeof (AliasingAllocOnNull);
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
		public void AliasingRecoveringReference()
		{
			var testedType = typeof (AliasingRecoveringReference);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(15),
						sameFile.WithLine(25),
					},
			}
			);
		}

		[TestMethod]
		public void GlobalTwiceEntry()
		{
			var testedType = typeof (GlobalTwiceEntry);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(19),
				Sliced =
					{
						sameFile.WithLine(19),
						sameFile.WithLine(17),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingFollowReferences()
		{
			var testedType = typeof (AliasingFollowReferences);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(22),
				Sliced =
					{
						//Alejandro cree que este slice tiene muchos falsos negativos
						//habría que hacer una segunda iteración de análisis, con
						//Christian/Maxi, y con Victor/Diego
						sameFile.WithLine(16),
						sameFile.WithLine(22),
					},
			}
			);
		}

		[TestMethod]
		public void AliasingReturningRefExpression()
		{
			var testedType = typeof (AliasingReturningRefExpression);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(15),
						sameFile.WithLine(20),
						sameFile.WithLine(31),
					},
			}
			);
		}

		[TestMethod]
		public void BindingInInvocation()
		{
			var testedType = typeof (ThisBindingInInvocation);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(15),
						sameFile.WithLine(30),
					},
			}
			);
		}

		[TestMethod]
		public void ActionOnFieldNeedingThisPrefix()
		{
			var testedType = typeof (ActionOnFieldNeedingThisPrefix);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(23),
						sameFile.WithLine(30),
						sameFile.WithLine(31),
						sameFile.WithLine(39),
					},
			}
			);
		}

		[TestMethod]
		public void ActionOnFieldThroughMemberAccess()
		{
			var testedType = typeof (ActionOnFieldThroughMemberAccess);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(23),
						sameFile.WithLine(28),
						sameFile.WithLine(36),
					},
			}
			);
		}

		[TestMethod]
		public void ArrayMultipleLastDef()
		{
			var testedType = typeof (ArrayMultipleLastDef);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(18),
				Sliced =
					{
						sameFile.WithLine(18),
						sameFile.WithLine(17),
						sameFile.WithLine(16),
						sameFile.WithLine(15),
						sameFile.WithLine(14),
						sameFile.WithLine(13),
					},
			}
			);
		}


		[TestMethod]
		public void HavocValueType()
		{
			var testedType = typeof (HavocValueType);
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
					},
			}
			);
		}

		[TestMethod]
		public void NonInstrumentedNestedOutside()
		{
			var testedType = typeof(NonInstrumentedNestedOutside);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(16),
				Sliced =
					{
						sameFile.WithLine(16),
						sameFile.WithLine(15),
						sameFile.WithLine(14),
					},
			}
			);
		}

		[TestMethod]
		public void NonInstrumentedNestedInside()
		{
			var testedType = typeof(NonInstrumentedNestedInside);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(15),
						sameFile.WithLine(14),
					},
			}
			);
		}

		[TestMethod]
		public void NonInstrumentedCallWithBinaryExpr()
		{
			Type testedType = typeof(NonInstrumentedCallWithBinaryExpr);
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
					},
			}
			);
		}

		[TestMethod]
		public void NonInstrumentedCallWithStringParam()
		{
			Type testedType = typeof(NonInstrumentedCallWithStringParam);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(22),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(16),
						sameFile.WithLine(17),
						sameFile.WithLine(19),
						sameFile.WithLine(20),
						sameFile.WithLine(22),
					},
			}
			);
		}

		[TestMethod]
		public void TypeOf()
		{
			var testedType = typeof(TypeOf);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(20),
				Sliced =
					{
						sameFile.WithLine(20),
						sameFile.WithLine(18),
						sameFile.WithLine(16),
						sameFile.WithLine(27),
						sameFile.WithLine(25),
						sameFile.WithLine(23),
						sameFile.WithLine(13),
					},
			}
			);
		}
		

	   

		[TestMethod]
		public void GetterWithReturn()
		{
			var testedType = typeof(GetterWithReturn);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(25),
						sameFile.WithLine(19),
					},
			}
			);
		}
		[TestMethod]
		public void MemberAccessChain()
		{
			var testedType = typeof(MemberAccessChainCase);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(18),
					},
			}
			);
		}

		[TestMethod]
		public void MemberAccessChainCombined()
		{
			var testedType = typeof(MemberAccessCombined);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(8),
				Sliced =
					{
						sameFile.WithLine(8),
						sameFile.WithLine(33),
					},
			}
			);
		}

		[TestMethod]
		public void InstrumentedPropertyDetect()
		{
			var testedType = typeof(InstrumentedPropertyDetect);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(23),
						sameFile.WithLine(24),
					},
			}
			);
		}

		[TestMethod]
		public void InlineNewInstrumented()
		{
			var testedType = typeof(InlineNewInstrumented);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(21),
						sameFile.WithLine(28),
						sameFile.WithLine(30),
					},
			}
			);
		}

		[TestMethod]
		public void InlineNewNonInstrumented()
		{
			var testedType = typeof(InlineNewNonInstrumented);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(18),
						sameFile.WithLine(19),
					},
			}
			);
		}

		[TestMethod]
		public void FieldInitializationAtDeclaration()
		{
			var testedType = typeof(FieldInitializationAtDeclaration);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(41),
					},
			}
			);
		}

		[TestMethod]
		public void LinqQueryToListWithCallBack()
		{
			var testedType = typeof(LinqQueryToListWithCallBack);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(18),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(16),
						sameFile.WithLine(17),
						sameFile.WithLine(18),
						sameFile.WithLine(23),
						sameFile.WithLine(25),
					},
			}
			);
		}

		[TestMethod]
		public void NonInstrumentedProperty()
		{
			var testedType = typeof(NonInstrumentedProperty);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(17),
				Sliced =
					{
						sameFile.WithLine(15),
						sameFile.WithLine(16),
						sameFile.WithLine(17),
					},
			}
			);
		}

		[TestMethod]
		public void ArrayInitialization()
		{
			var testedType = typeof(ArrayInitialization);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(15),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(19),
					},
			}
			);
		}

		[TestMethod]
		public void NotSupportedForeach()
		{
			var testedType = typeof (NotSupportedForeach);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(25),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(16),
						sameFile.WithLine(17),
						sameFile.WithLine(18),
						sameFile.WithLine(19),
						sameFile.WithLine(20),
						sameFile.WithLine(21),
						sameFile.WithLine(23),
						sameFile.WithLine(25),
						sameFile.WithLine(29),
						sameFile.WithLine(31),
					},
			}
			);
		}

		[TestMethod]
		public void NotSupportedLambda()
		{
			var testedType = typeof(NotSupportedLambda);
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
		public void ListWithForeach()
		{
			var testedType = typeof(ListWithForeach);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(17),
				Sliced =
					{
						sameFile.WithLine(17),
						sameFile.WithLine(14),
						sameFile.WithLine(13),
					},
			}
			);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(16),
				Sliced =
					{
						sameFile.WithLine(16),
						sameFile.WithLine(15),
						//esta linea no se slicean porque list.Add no modifica el puntero sino que solamente el objeto interno (en el aPtG)
						//sameFile.WithLine(14),
						sameFile.WithLine(13),
					},
			}
			);
		}

		[TestMethod]
		public void ForeachWithAssignment()
		{
			var testedType = typeof(ForeachWithAssignment);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				//este criteria trae lineas que asignan la VARIABLE
				//aunque no se modifiquen los campos
				Criteria = sameFile.WithLine(21),
				Sliced =
					{
						sameFile.WithLine(21),
						sameFile.WithLine(18),
						sameFile.WithLine(16),
						sameFile.WithLine(13),
					}
			}
			);
			TestSimpleSlice(testedType, new TestResult
			{
				//este criteria trae lineas que asignan los campos
				//aunque no se modifique las variable
				Criteria = sameFile.WithLine(20),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(20),
						sameFile.WithLine(25),
						sameFile.WithLine(27),
					},
			}
			);
		}

		[TestMethod]
		public void LockPassthrough()
		{
			var testedType = typeof(LockPassthrough);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(16),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(15),
						sameFile.WithLine(16),
						sameFile.WithLine(23),
						sameFile.WithLine(28),
					},
			}
			);
		}
		
		[TestMethod]
		public void AliasingThisLikeParameter()
		{
			var testedType = typeof(AliasingThisLikeParameter);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(9),
				Sliced =
					{
						sameFile.WithLine(9),
						sameFile.WithLine(16),
						sameFile.WithLine(18),
						sameFile.WithLine(7),
						sameFile.WithLine(14),
					},
			}
			);
		}

		[TestMethod]
		public void OrExpressionWithTrue()
		{
			var testedType = typeof(OrExpressionWithTrue);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(17),
				Sliced =
					{
						sameFile.WithLine(17),
						sameFile.WithLine(16),
						sameFile.WithLine(15),
						sameFile.WithLine(13),
					},
			}
			);
		}

		[TestMethod]
		public void NestedClasses()
		{
			var testedType = typeof(NestedClasses);
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
		public void LiteralToObjectBinding()
		{
			var testedType = typeof(LiteralToObjectBinding);
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
		public void RefParamInsideInvocation()
		{
			var testedType = typeof(RefParamInsideInvocation);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(14),
				Sliced =
					{
						sameFile.WithLine(14),
						sameFile.WithLine(26),
					},
			}
			);
		}

		[TestMethod]
		public void GoToStatement()
		{
			var testedType = typeof(GoToStatement);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(13),
				Sliced =
					{
						sameFile.WithLine(13),
						sameFile.WithLine(21),
						sameFile.WithLine(22),
					},
			}
			);
		}

		[TestMethod]
		public void TryCatchStatement()
		{
			var testedType = typeof(TryCatchStatement);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(23),
				Sliced =
					{
						sameFile.WithLine(23),
						sameFile.WithLine(16),
						sameFile.WithLine(13),
						sameFile.WithLine(30),
					},
			}
			);
		}

		[TestMethod]
		public void InheritanceCallBase()
		{
			var testedType = typeof(InheritanceCallBase);
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
		public void InheritanceCallBaseWithFunction()
		{
			var testedType = typeof(InheritanceCallBaseWithFunction);
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
		public void InheritanceImplicitCallBase()
		{
			var testedType = typeof(InheritanceImplicitCallBase);
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
		public void InheritanceCallBaseMultipleCtors()
		{
			var testedType = typeof(InheritanceCallBaseMultipleCtors);
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
		public void FormRepresentation()
		{
			var testedType = typeof(FormRepresentation);
			var sameFile = SameFileStmtBuilder(testedType);
			TestSimpleSlice(testedType, new TestResult
			{
				Criteria = sameFile.WithLine(21),
				Sliced =
					{
						sameFile.WithLine(21),
					},
			}
			);
		}
	}
}