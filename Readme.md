ODataGenerator
=================

ODataGenerator is a DSL package (a plugin module) for [Rhetos development platform](https://github.com/Rhetos/Rhetos).

ODataGenerator automatically generates OData interface (Open Data Protocol) for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

Features
========

Current OData interface only supports query operations.

* Expand is not supported.
* Writeable interface is not supported.

Usage examples:

1. To query data from entity *Claim* in module *Common* with Rhetos service running on *localhost/Rhetos*, use URI:

        http://localhost/Rhetos/OData/CommonClaim     
2. To import data from OData service to Excel 2013, select
   Data -> Get External Data -> From Other Sources -> From OData Data Feed -> paste the link above -> Next -> check CommonClaim -> Finish -> Ok.
	* This table in the Excel document is linked to the Rhetos server. By clicking Refresh you can retrieve new data into the Excel table. 

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

The generated web service uses *Microsoft WCF Data Services 5.0*. The Data Services dlls are automatically deployed with the ODataGenerator package.
