# ODataGenerator

ODataGenerator is a plugin package for [Rhetos development platform](https://github.com/Rhetos/Rhetos).
It automatically generates OData interface (Open Data Protocol) for all entities and other queryable data structures that are defined in a Rhetos application.

See [rhetos.org](http://www.rhetos.org/) for more information on Rhetos.

## Features

Current OData interface only supports query operations.

* Expand is not supported.
* Writeable interface is not supported.

Usage examples, for Rhetos service running on `http://localhost/Rhetos`:

1. Using **web browser**, query data from entity *Claim* in module *Common* by opening URI: `http://localhost/Rhetos/OData/CommonClaim`
2. To import data from OData service to **Excel 2013**, use menu
   Data -> Get External Data -> From Other Sources -> From OData Data Feed -> paste the link above -> Next -> check CommonClaim -> Finish -> Ok.
   * This table in the Excel document is linked to the Rhetos server. By clicking Refresh you can retrieve new data into the Excel table.
3. Using **LinqPad**: Click *Add connection*, choose *WCF Data Services (OData)*
   and enter URI `http://localhost/Rhetos/OData`.
   After reading the service metadata, LinqPad will show all available entities.
   Right-click on any entity to start querying.

## Build

To build the package from source, run `Build.bat`.
The script will pause in case of an error.
The build output is a NuGet package in the "Install" subfolder.

## Installation

To install this package to a Rhetos server, add it to the Rhetos server's *RhetosPackages.config* file
and make sure the NuGet package location is listed in the *RhetosPackageSources.config* file.

* The package ID is "**Rhetos.ODataGenerator**".
* For more information, see [Installing plugin packages](https://github.com/Rhetos/Rhetos/wiki/Installing-plugin-packages).
