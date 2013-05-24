@echo off

set config=Debug
set dependency_path=.\Dependencies\
set cci_namespace=Microsoft.Cci
set cci_path=C:\CCI\
set cci_ast_path=%cci_path%\Sources\%%f\bin\%config%\
set cci_metadata_path=%cci_path%\Metadata\Sources\%%f\bin\%config%\

set ast_files=
set ast_files=%ast_files% AssertionAdder
set ast_files=%ast_files% AstsProjectedAsCodeModel
set ast_files=%ast_files% CodeModel
set ast_files=%ast_files% CodeModelToIL
set ast_files=%ast_files% ContractExtractor
set ast_files=%ast_files% ILToCodeModel
set ast_files=%ast_files% MutableCodeModel
set ast_files=%ast_files% NewILToCodeModel

set metadata_files=
set metadata_files=%metadata_files% ILGenerator
set metadata_files=%metadata_files% MetadataHelper
set metadata_files=%metadata_files% MetadataModel
set metadata_files=%metadata_files% MutableMetadataModel
set metadata_files=%metadata_files% PdbReader
set metadata_files=%metadata_files% PdbWriter
set metadata_files=%metadata_files% PeReader
set metadata_files=%metadata_files% PeWriter
set metadata_files=%metadata_files% SourceModel

echo Copying files:

for %%f in (%ast_files%) do call :copy "%cci_ast_path%\%cci_namespace%.%%f.dll" "%%f"
for %%f in (%metadata_files%) do call :copy "%cci_metadata_path%\%cci_namespace%.%%f.dll" "%%f"

pause
goto :eof

:copy
((@copy /y %1 "%dependency_path%" > nul) && (echo        %~2)) || (echo Failed %~2)
goto :eof
