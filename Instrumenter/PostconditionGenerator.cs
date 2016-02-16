using System;
using System.Collections.Generic;
using System.Linq;
using Analysis.Cci;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace Instrumenter
{
    public class PostconditionGenerator
    {
        public List<Postcondition> GeneratePostconditions(FieldDefinition field, Dictionary<string, List<string>> transitions)
        {
            throw new NotImplementedException();
            //return transitions.Select(t => GeneratePostcondition(field, t.Key, t.Value)).ToList();
        }

        //protected Postcondition GeneratePostcondition(FieldDefinition field, string fromId, IEnumerable<string> to)
        //{
        //    var host = CciHostEnvironment.GetInstance();
        //    IExpression conditional;

        //    if (this.epa.Initial.Name == fromId)
        //    {
        //        //Initial state
        //        conditional = GenerateConditionPartTo(field, to);
        //    }
        //    else
        //    {
        //        conditional = new Conditional()
        //        {
        //            Type = host.PlatformType.SystemBoolean,
        //            Condition = GenerateConditionPartFrom(field, fromId),
        //            ResultIfTrue = GenerateConditionPartTo(field, to),
        //            ResultIfFalse = new CompileTimeConstant()
        //            {
        //                Type = host.PlatformType.SystemBoolean,
        //                Value = true
        //            }
        //        };
        //    }

        //    return new Postcondition()
        //    {
        //        Condition = conditional,
        //        OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(conditional)
        //    };
        //}

        //protected IExpression GenerateConditionPartFrom(FieldDefinition field, string fromId)
        //{
        //    var host = CciHostEnvironment.GetInstance();
        //    return new Equality()
        //    {
        //        Type = host.PlatformType.SystemBoolean,
        //        LeftOperand = new OldValue()
        //        {
        //            Type = field.Type,
        //            Expression = new BoundExpression()
        //            {
        //                Definition = field,
        //                Instance = new ThisReference(),
        //                Type = field.Type
        //            }
        //        },
        //        RightOperand = new CompileTimeConstant()
        //        {
        //            Type = field.Type,
        //            Value = fromId
        //        }
        //    };
        //}

        //protected IExpression GenerateConditionPartTo(FieldDefinition field, IEnumerable<string> to)
        //{
        //    var host = CciHostEnvironment.GetInstance();
        //    var conditions = new List<IExpression>();

        //    foreach (var toId in to)
        //    {
        //        var cond = new Equality()
        //        {
        //            Type = host.PlatformType.SystemBoolean,
        //            LeftOperand = new BoundExpression()
        //            {
        //                Definition = field,
        //                Instance = new ThisReference(),
        //                Type = field.Type
        //            },
        //            RightOperand = new CompileTimeConstant()
        //            {
        //                Type = field.Type,
        //                Value = toId
        //            }
        //        };

        //        conditions.Add(cond);
        //    }

        //    return Helper.JoinWithLogicalOr(host, conditions, false);
        //}

    }
}