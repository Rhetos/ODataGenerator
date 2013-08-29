/*
    Copyright (C) 2013 Omega software d.o.o.

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
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Xml;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.ODataGenerator;

namespace Rhetos.ODataGenerator.DefaultConcepts
{
    [Export(typeof(IODataGeneratorPlugin))]
    [ExportMetadata(MefProvider.Implements, typeof(PropertyInfo))]
    public class SimplePropertyCodeGenerator : IODataGeneratorPlugin
    {

        private static string GetPropertyType(PropertyInfo conceptInfo)
        {
            if (typeof(IntegerPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "int?";
            if (typeof(BinaryPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "byte[]";
            if (typeof(BoolPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "bool?";
            if (typeof(DatePropertyInfo).IsAssignableFrom(conceptInfo.GetType()) 
                || typeof(DateTimePropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "DateTime?";
            if (typeof(MoneyPropertyInfo).IsAssignableFrom(conceptInfo.GetType()) 
                || typeof(DecimalPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "decimal?";
            if (typeof(GuidPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "Guid?";
            if (typeof(ShortStringPropertyInfo).IsAssignableFrom(conceptInfo.GetType()) 
                || typeof(LongStringPropertyInfo).IsAssignableFrom(conceptInfo.GetType())) return "string";
            return null;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            PropertyInfo info = (PropertyInfo)conceptInfo;
            string propertyType = GetPropertyType(info);
            if (!String.IsNullOrEmpty(propertyType) && DataStructureCodeGenerator.IsTypeSupported(info.DataStructure))
            {
                ODataPropertyHelper.GenerateCodeForType(info, codeBuilder, propertyType);
            }
        }

    }
}