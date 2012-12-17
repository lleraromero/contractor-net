using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci;

namespace Contractor.Core
{
	//TODO: borrar esta clase de prueba
	class MyCodeMutator : MethodBodyCodeMutator
	{
		public MyCodeMutator(IMetadataHost host) : base(host)
		{
		}

		public override IMethodDefinition Visit(IMethodDefinition methodDefinition)
		{
			return base.Visit(methodDefinition);
		}

		public override IExpression Visit(BitwiseAnd bitwiseAnd)
		{
			return base.Visit(bitwiseAnd);
		}

		public override IExpression Visit(Conditional conditional)
		{
			return base.Visit(conditional);
		}

		public override IExpression Visit(BinaryOperation binaryOperation)
		{
			return base.Visit(binaryOperation);
		}

		public override IStatement Visit(ConditionalStatement conditionalStatement)
		{
			return base.Visit(conditionalStatement);
		}
	}
}
