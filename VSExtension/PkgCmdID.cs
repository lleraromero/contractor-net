// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace Contractor.VSExtension
{
    static class PkgCmdIDList
    {
		public const int ContractExplorerToolbar = 0x0100;

		public const int ContractExplorerToolbarGroup1 = 0x0200;
		public const int ContractExplorerToolbarGroup2 = 0x0201;
		public const int ContractExplorerToolbarGroup3 = 0x0202;
		public const int ContractExplorerToolbarGroup4 = 0x0203;
		public const int ContractExplorerToolbarGroup5 = 0x0204;

		public const int ContractorExplorerButton = 0x0300;
		public const int StopAnalysisButton = 0x0301;
		public const int ZoomInButton = 0x0302;
		public const int ZoomOutButton = 0x0303;
		public const int ZoomBestFitButton = 0x0304;
		public const int PanButton = 0x0305;
		public const int RefreshButton = 0x0306;
		public const int ExportGraphButton = 0x0307;
		public const int GenerateOutputAssemblyButton = 0x0308;
		public const int ResetLayoutButton = 0x0309;
		public const int UndoButton = 0x0310;
		public const int RedoButton = 0x0311;
		public const int OptionPageButton = 0x0312;
    };
}