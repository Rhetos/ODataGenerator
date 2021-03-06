﻿/*
    Copyright (C) 2014 Omega software d.o.o.

    This file is part of Rhetos.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Rhetos.Compiler;
using Rhetos.Extensibility;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using ICodeGenerator = Rhetos.Compiler.ICodeGenerator;

namespace Rhetos.ODataGenerator
{
    [Export(typeof(IGenerator))]
    public class ODataGenerator : IGenerator
    {
        private readonly IPluginsContainer<IODataGeneratorPlugin> _plugins;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IAssemblyGenerator _assemblyGenerator;
        private const string assemblyName = "ODataService";

        public ODataGenerator(
            IPluginsContainer<IODataGeneratorPlugin> plugins,
            ICodeGenerator codeGenerator,
            IAssemblyGenerator assemblyGenerator
        )
        {
            _plugins = plugins;
            _codeGenerator = codeGenerator;
            _assemblyGenerator = assemblyGenerator;
        }

        public void Generate()
        {
            IAssemblySource assemblySource = _codeGenerator.ExecutePlugins(_plugins, "/*", "*/", new InitialCodeGenerator());
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                OutputAssembly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Generated", assemblyName + ".dll"),
                IncludeDebugInformation = true,
                CompilerOptions = "/optimize"
            };
            _assemblyGenerator.Generate(assemblySource, parameters);
        }

        public IEnumerable<string> Dependencies => null;
    }
}
