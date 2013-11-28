ODataGenerator
=================

ODataGenerator is a DSL package (a plugin module) for [Rhetos development platform](https://github.com/Rhetos/Rhetos).

ODataGenerator automatically generates OData interface (Open Data Protocol) for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

Features
========

Current OData interface only support query operations.

* Expand is not supported.
* Writeable interface is not supported.

Prerequisites
=============

Utilities in this project are based on relative path to Rhetos repository. [Rhetos source](https://github.com/Rhetos/Rhetos) must be downloaded to a folder with relative path `..\..\Rhetos`. 

Sample folder structure:
 
	\ROOT
		\Rhetos
		\RhetosPackages
			\ODataGenerator


Build and Installation
======================

Build package with `Build.bat`. Check BuildError.log for errors.

Instalation package creation:

1. Set the new version number in `ChangeVersion.bat` and start it.
2. Start `CreatePackage.bat`. Instalation package (.zip) is going to be created in parent directory of ODataGenerator.

The generated web service uses *Microsoft WCF Data Services 5.0* (it comes with Visual Studio 2012). The Data Services dlls are included with the ODataGenerator package. 