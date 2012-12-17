using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API_Examples;
using System.Diagnostics.Contracts;

namespace Client_Examples
{
    class StackScenarios
    {
        public void NormalScenario()
        {
            var stack = new EnrichedStackSimple();

            stack.Push();
            stack.Pop();
        }

        public void NoDeterminismScenario()
        {
            var stack = new EnrichedStackSimple();
            Contract.Assert(stack.count == 0);

            stack.Push(); Contract.Assert(stack.count == 1);
            stack.Push(); Contract.Assert(stack.count == 2);
            stack.Push(); Contract.Assert(stack.count == 3);
            stack.Push(); Contract.Assert(stack.count == 4);
            stack.Push(); Contract.Assert(stack.count == 5);
            stack.Push(); // full stack: push not allowed
        }

        public void InvalidPopScenario()
        {
            var stack = new EnrichedStackSimple();

            stack.Pop();
            stack.Push();
        }

        public void InvalidPushScenario()
        {
            var stack = new EnrichedStackSimple();
            Contract.Assert(stack.count == 0);

            stack.Push(); Contract.Assert(stack.count == 1);
            stack.Push(); Contract.Assert(stack.count == 2);
            stack.Pop(); Contract.Assert(stack.count == 1);
            stack.Push(); Contract.Assert(stack.count == 2);
            stack.Push(); Contract.Assert(stack.count == 3);
            stack.Push(); Contract.Assert(stack.count == 4);
            stack.Push(); Contract.Assert(stack.count == 5);
            stack.Push(); // full stack: push not allowed
        }
    }
}
