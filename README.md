# Contractor.NET
## A .NET validation and specification strengthening tool

![finitestack.png](https://bitbucket.org/repo/BxkMnd/images/3992330483-finitestack.png)

Contractor.NET is a tool developed to construct contract specifications with typestate information which can be used for verification of client code. Contractor.NET uses and extends Code Contracts to provide stronger contract specifications.

It features a two step process: first, a class source code is analyzed to extract a finite state behavior model (in the form of a typestate) that is amenable to human-in-the-loop validation and refinement. The second step is to augment the original contract specification for the input class with the inferred typestate information, therefore enabling the verification of client code.

The inferred typestates are enabledness preserving: a level of abstraction that has been successfully used to validate software artifacts, assisting in the detection of a number of concerns in various case studies including specifications of Microsoft Server protocols.

Contractor.NET is based on [Contractor](http://lafhis.dc.uba.ar/misc/contractor/Welcome.html), a previous work by the same authors.

# Using Contractor.NET

There are different ways to use Contractor.NET:

### Contractor.NET on the command line

### Contractor.NET GUI

# Getting Contractor.NET

The current release of Contractor.NET is available for download at 
<https://bitbucket.org/lleraromero/contractor-net/get/master.zip>.

To access the source code, use the bitbucket repository:

```git clone https://lleraromero@bitbucket.org/lleraromero/contractor-net.git```

By default, the application should work properly using the compiled Corral version included in this repository. It is currently Z3 x86 v4.3.1.

To use Code Contracts as a backend, we have tested it with version 1.9.10714.2.


# Dependencies 
In order to build Contractor.NET a few dependencies are needed. Some of them come as NuGet packages and the rest are available in this repository.

However, if you need to debug the code you should grab the source code for this dependencies:

* Corral
```
git clone https://github.com/boogie-org/corral.git
git checkout 0a37e8f
```

* Boogie
```
git clone https://github.com/boogie-org/boogie.git
git checkout 5bb7ad4
```

* Z3
```
git clone https://github.com/Z3Prover/z3.git
git checkout 89c1785
```

* Bytecode Translator
```
git clone https://github.com/lleraromero/bytecodetranslator.git
git checkout c397a3e
```
Eventually, this should be merged with the official repository.

* CCI AST
Download from: [http://cciast.codeplex.com/](http://cciast.codeplex.com/) (Some changes weren't uploaded yet, since we are waiting for a git repository)

* Code Contracts
```git clone https://github.com/Microsoft/CodeContracts.git```

# More Information
Website: http://lafhis.dc.uba.ar/contractor/contractor.net-web/

Slides: [ACorralando EPAs: acercando el modelo mental al computacional](https://speakerdeck.com/lleraromero/acorralando-epas-acercando-el-modelo-mental-al-computacional)