using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Contractor.Core
{
    public enum ResultKind { TrueBug, NoBugs, RecursionBoundReached }
    class CorralRunner
    {
        protected CancellationToken token;
        protected Query result;
        public Query Result { get { return result; } }
        public CorralRunner(CancellationToken token)
        {
            Contract.Requires(token != null);

            this.token = token;
        }

        public TimeSpan Run(string args, Query query)
        {
            Contract.Requires(!string.IsNullOrEmpty(args));

            // Check if the user stopped the analysis
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
                    this.result = new ReachableQuery(query.Action);
                    break;
                case cba.CorralResult.NoBugs:
                    this.result = new UnreachableQuery(query.Action);
                    break;
                case cba.CorralResult.RecursionBoundReached:
                    this.result = new MayBeReachableQuery(query.Action);
                    break;
                default:
                    throw new NotImplementedException("The result was not understood");
            }

            return timer.Elapsed;
        }
    }
}
