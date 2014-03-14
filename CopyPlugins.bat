ECHO Target folder = [%1]
ECHO $(ConfigurationName) = [%2]

SET ThisScriptFolder="%~dp0"

XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Rhetos.ODataGenerator.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Rhetos.ODataGenerator.pdb %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator.DefaultConcepts\bin\%2\Rhetos.ODataGenerator.DefaultConcepts.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator.DefaultConcepts\bin\%2\Rhetos.ODataGenerator.DefaultConcepts.pdb %1 || EXIT /B 1

XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Edm.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Edm.xml %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.OData.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.OData.xml %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Services.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Services.xml %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Services.Client.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Microsoft.Data.Services.Client.xml %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\System.Spatial.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\System.Spatial.xml %1 || EXIT /B 1

XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\System.Web.Mvc.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Packages\ASP.NET\System.Web.Razor.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Packages\ASP.NET\System.Web.WebPages.Razor.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Packages\ASP.NET\System.Web.WebPages.dll %1 || EXIT /B 1

EXIT /B 0
