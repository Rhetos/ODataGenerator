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
    [ExportMetadata(MefProvider.Implements, typeof(DataStructureInfo))]
    public class DataStructureCodeGenerator : IODataGeneratorPlugin
    {
        public static readonly CsTag<DataStructureInfo> ClonePropertiesTag = "OData.CloneProperties";
        public static readonly CsTag<DataStructureInfo> ClonePropertiesInitializationTag = "OData.ClonePropertiesInitialization";
        
        private static string ImplementationCodeSnippet(DataStructureInfo info)
        {
            return string.Format(@"
    namespace {0} 
    {{ 
        [DataServiceKey(""ID"")]
        [DataServiceEntity]
        [System.Data.Services.Common.EntitySetAttribute(""{0}.{1}"")]
        public partial class {1}
        {{
            public virtual Guid ID {{ get; set; }}
            {2}

            public static IQueryable<Rhetos.OData.{0}.{1}> {4}(Rhetos.Processing.IProcessingEngine processingEngine)
            {{
                try {{
                    var commandsResult = processingEngine.Execute(new[] {{ new global::Rhetos.OData.DefaultCommands.ODataQueryCommandInfo {{ DataSource = ""{0}.{1}"" }} }});
                    if (!commandsResult.Success)
                        throw new Rhetos.UserException(commandsResult.UserMessage ?? commandsResult.SystemMessage, commandsResult.SystemMessage);

                    var result = (global::Rhetos.OData.DefaultCommands.ODataQueryCommandResult)commandsResult.CommandResults.Single().Data;
                    var transformedResult = ((IQueryable<global::{0}.{1}>)result.Query).Select(e => new Rhetos.OData.{0}.{1}
                    {{
                        ID = e.ID{3}
                    }});
                    return transformedResult;
                }} catch (Exception ex)
                {{
                    // unable to use filter-free IQueryable
                    return (new []
                        {{ new Lazy<Rhetos.OData.{0}.{1}>(() => {{ throw new DataServiceException(ex.Message, ex); }} )
                        }}).AsQueryable().Select(item => item.Value);
                }}
            }}
        }}
    }}

    ",
            info.Module.Name, 
            info.Name, 
            ClonePropertiesTag.Evaluate(info),
            ClonePropertiesInitializationTag.Evaluate(info),
            "Get" + info.Name + "Queryable");
        }

        private static string DataSourceExpositionCodeSnippet(DataStructureInfo info)
        {
            return string.Format(@"
        public IQueryable<Rhetos.OData.{0}.{1}> {0}{1}
        {{
            get
            {{
                return Rhetos.OData.{0}.{1}.Get{1}Queryable(_processingEngine);
            }}
        }}",
            info.Module.Name,
            info.Name,
            "_" + info.Module.Name.ToLower() + info.Name);
        }

        private static bool _isInitialCallMade;

        public static bool IsTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is EntityInfo
                || conceptInfo is BrowseDataStructureInfo
                || conceptInfo is LegacyEntityInfo
                || conceptInfo is LegacyEntityWithAutoCreatedViewInfo
                || conceptInfo is SqlQueryableInfo
                || conceptInfo is ComputedInfo
                || conceptInfo is QueryableExtensionInfo;
        }

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            DataStructureInfo info = (DataStructureInfo)conceptInfo;

            if (IsTypeSupported(info))
            {
                GenerateInitialCode(codeBuilder);

                codeBuilder.InsertCode(ImplementationCodeSnippet(info), InitialCodeGenerator.NamespaceMembersTag);

                codeBuilder.InsertCode(DataSourceExpositionCodeSnippet(info), InitialCodeGenerator.DataSourceExpositionMembersTag);
            }
        }

        private static void GenerateInitialCode(ICodeBuilder codeBuilder)
        {
            if (_isInitialCallMade)
                return;
            _isInitialCallMade = true;
        }
    }
}