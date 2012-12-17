// Guids.cs
// MUST match guids.h
using System;

namespace Contractor.VSExtension
{
    static class GuidList
    {
        public const string guidVSExtensionPkgString = "6fcfab98-b090-4f07-9c9d-e4d16e08a4fd";
        public const string guidVSExtensionCmdSetString = "42270417-3a29-46a6-962c-f8a5b881f279";
        public const string guidToolWindowPersistanceString = "b5c59e6b-e696-4bfe-8c0a-c75f4524671d";
		public const string guidContractorExplorerString = "b5c59e6b-e696-4bfe-8c0a-c75f4524671d";
		public const string guidOptionsString = "46474834-5059-4769-b632-7719b2ce98b1";

        public static readonly Guid guidVSExtensionCmdSet = new Guid(guidVSExtensionCmdSetString);
    };
}