ECHO Target folder = [%1]
ECHO $(ConfigurationName) = [%2]

SET ThisScriptFolder=%~dp0

XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Rhetos.ODataGenerator.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator\bin\%2\Rhetos.ODataGenerator.pdb %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator.DefaultConcepts\bin\%2\Rhetos.ODataGenerator.DefaultConcepts.dll %1 || EXIT /B 1
XCOPY /Y/D/R %ThisScriptFolder%Plugins\Rhetos.ODataGenerator.DefaultConcepts\bin\%2\Rhetos.ODataGenerator.DefaultConcepts.pdb %1 || EXIT /B 1

EXIT /B 0
