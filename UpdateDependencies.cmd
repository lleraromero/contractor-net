@echo off

echo Copying CCI-AST:
set cci_path=D:\cciast\bin\Debug
set dependency_path=.\Dependencies\CCI-AST

set files=
set files=%files% Microsoft.Cci.Analysis.AnalysisUtilities
set files=%files% Microsoft.Cci.Analysis.ControlAndDataFlowGraph
set files=%files% Microsoft.Cci.AstsProjectedAsCodeModel
set files=%files% Microsoft.Cci.CodeModel
set files=%files% Microsoft.Cci.CodeModelToIL
set files=%files% Microsoft.Cci.ContractExtractor
set files=%files% Microsoft.Cci.CSharpSourceEmitter
set files=%files% Microsoft.Cci.ILGenerator
set files=%files% Microsoft.Cci.MetadataHelper
set files=%files% Microsoft.Cci.MetadataModel
set files=%files% Microsoft.Cci.MutableCodeModel
set files=%files% Microsoft.Cci.MutableMetadataModel
set files=%files% Microsoft.Cci.NewILToCodeModel
set files=%files% Microsoft.Cci.PdbReader
set files=%files% Microsoft.Cci.PdbWriter
set files=%files% Microsoft.Cci.PeReader
set files=%files% Microsoft.Cci.PeWriter
set files=%files% Microsoft.Cci.ReflectionEmitter
set files=%files% Microsoft.Cci.SourceEmitter
set files=%files% Microsoft.Cci.SourceModel

for %%f in (%files%) do call :copy "%cci_path%\%%f.*" "%%f"

echo Copying BCT:
set bct_path=D:\bct\Binaries
set dependency_path=.\Dependencies\BCT

for %%f in (%bct_path%\*.*) do call :copy "%%f" "%%f"

echo Copying Corral:
set corral_path=D:\corral\bin\Debug
set dependency_path=.\Dependencies\Corral

for %%f in (%corral_path%\*.*) do call :copy "%%f" "%%f"

pause
goto :eof

:copy
((@copy /y %1 "%dependency_path%" > nul) && (echo        %~2)) || (echo Failed %~2)
goto :eof
