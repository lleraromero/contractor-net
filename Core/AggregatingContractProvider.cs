using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.Contracts;
using Microsoft.Cci;

namespace Contractor.Utils
{
	class AggregatingContractProvider : ContractProvider
	{
		private IContractExtractor underlyingContractExtractor;

		public AggregatingContractProvider(IContractExtractor contractExtractor)
			: base(contractExtractor.ContractMethods, contractExtractor.Unit)
		{
			underlyingContractExtractor = contractExtractor;
		}

		public override ILoopContract GetLoopContractFor(object loop)
		{
			var loopContract = base.GetLoopContractFor(loop);

			if (loopContract == null)
			{
				loopContract = underlyingContractExtractor.GetLoopContractFor(loop);
				//base.AssociateLoopWithContract(loop, loopContract);
			}

			return loopContract;
		}

		public override IMethodContract GetMethodContractFor(object method)
		{
			var methodContract = base.GetMethodContractFor(method);

			if (methodContract == null)
			{
				methodContract = underlyingContractExtractor.GetMethodContractFor(method);
				//base.AssociateMethodWithContract(method, methodContract);
			}

			return methodContract;
		}

		public override IEnumerable<IEnumerable<IExpression>> GetTriggersFor(object quantifier)
		{
			var triggers = base.GetTriggersFor(quantifier);

			if (triggers == null)
			{
				triggers = underlyingContractExtractor.GetTriggersFor(quantifier);
				//base.AssociateTriggersWithQuantifier(quantifier, triggers);
			}

			return triggers;
		}

		public override ITypeContract GetTypeContractFor(object type)
		{
			var typeContract = base.GetTypeContractFor(type);

			if (typeContract == null)
			{
				typeContract = underlyingContractExtractor.GetTypeContractFor(type);
				//base.AssociateTypeWithContract(type, typeContract);
			}

			return typeContract;
		}
	}
}
