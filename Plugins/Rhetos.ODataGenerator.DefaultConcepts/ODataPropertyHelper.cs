/*
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
using Rhetos.Dsl.DefaultConcepts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rhetos.ODataGenerator.DefaultConcepts
{

    public static class ODataPropertyHelper
    {
        public static readonly CsTag<PropertyInfo> AttributeTag = "Attribute";

        private static string DeclarationCodeSnippet(PropertyInfo info, string type, string nameSuffix)
        {
            return string.Format(@"
            " + AttributeTag.Evaluate(info) + @"
            public virtual {1} {0}{2} {{ get; set; }}
        ", info.Name, type, nameSuffix);
        }

        private static string InitializationCodeSnippet(PropertyInfo info, string nameSuffix)
        {
            return string.Format(@",
                        {0} = e.{1}", info.Name+nameSuffix, string.IsNullOrEmpty(nameSuffix)?info.Name:info.Name + "." + nameSuffix);
        }

        public static void GenerateCodeForType(PropertyInfo info, ICodeBuilder codeBuilder, string type, string nameSuffix = "")
        {
            codeBuilder.InsertCode(DeclarationCodeSnippet(info, type, nameSuffix), DataStructureCodeGenerator.ClonePropertiesTag, info.DataStructure);
            codeBuilder.InsertCode(InitializationCodeSnippet(info, nameSuffix), DataStructureCodeGenerator.ClonePropertiesInitializationTag, info.DataStructure);
        }
    }
}
