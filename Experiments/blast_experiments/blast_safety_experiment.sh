#!/bin/bash

#Adding pblast.opt to the PATH
export PATH=$PATH:$(readlink -f blast-2.5)

#Test cases to compare to Corral
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloMasChicoQueRecursionBound.png CicloMasChicoQueRecursionBound.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloMasLargoQueRecursionBound.png CicloMasLargoQueRecursionBound.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloBasadoEnParam.png CicloBasadoEnParam.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloDeberiaIrA10.png CicloDeberiaIrA10.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloNoDeberiaIrA10.png CicloNoDeberiaIrA10.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloDeberiaIrA10ConIf.png CicloDeberiaIrA10ConIf.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f CicloNoDeberiaIrA10ConIf.png CicloNoDeberiaIrA10ConIf.binding

#ICSE Examples
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f List.png List.binding
python contractor_0.41/contractor.py -s -i code-with-pre -o png -f ListItr.png ListItr.binding
