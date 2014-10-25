# Contractor.NET | A .NET validation and specification strengthening tool #

Contractor.NET is a tool developed to construct contract specifications with typestate information which can be used for verification of client code. Contractor.NET uses and extends Code Contracts to provide stronger contract specifications.

It features a two step process: first, a class source code is analyzed to extract a finite state behavior model (in the form of a typestate) that is amenable to human-in-the-loop validation and refinement. The second step is to augment the original contract specification for the input class with the inferred typestate information, therefore enabling the verification of client code.

The inferred typestates are enabledness preserving: a level of abstraction that has been successfully used to validate software artifacts, assisting in the detection of a number of concerns in various case studies including specifications of Microsoft Server protocols.

Contractor.NET is based on [Contractor](http://lafhis.dc.uba.ar/misc/contractor/Welcome.html), a previous work by the same authors.