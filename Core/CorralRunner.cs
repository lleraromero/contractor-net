using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contractor.Core
{
    public enum ResultKind { TrueBug, NoBugs, RecursionBoundReached }
    class CorralRunner
    {
        protected CancellationToken token;
        protected ResultKind result;
        public ResultKind Result { get { return result; } }
        public CorralRunner(CancellationToken token)
        {
            Contract.Requires(token != null);

            this.token = token;
        }

        public TimeSpan Run(string args)
        {
            Contract.Requires(!string.IsNullOrEmpty(args));

            token.ThrowIfCancellationRequested();

            var timer = Stopwatch.StartNew();

            var corral = new cba.Driver();
            try
            {
                if (corral.run(args.Split(' ')) != 0)
                {
                    throw new Exception("Error executing corral");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Fatal, ex);
                Logger.Log(LogLevel.Info, args);
                throw;
            }

            timer.Stop();

            switch (corral.Result)
            {
                case cba.CorralResult.BugFound:
                    this.result = ResultKind.TrueBug;
                    break;
                case cba.CorralResult.NoBugs:
                    this.result = ResultKind.NoBugs;
                    break;
                case cba.CorralResult.RecursionBoundReached:
                    this.result = ResultKind.RecursionBoundReached;
                    break;
                default:
                    throw new NotImplementedException("The result was not understood");
            }

            return timer.Elapsed;
        }
    }
}
