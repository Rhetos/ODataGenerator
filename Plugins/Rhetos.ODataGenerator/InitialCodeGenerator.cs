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
using Rhetos;
using Rhetos.Compiler;
using Rhetos.Dsl;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;

namespace Rhetos.ODataGenerator
{
    public class InitialCodeGenerator : IODataGeneratorPlugin
    {
        public const string UsingTag = "/*using*/";

        public const string NamespaceMembersTag = "/*implementationBody*/";

        public const string DataSourceExpositionMembersTag = "/*entityDataSourcesDeclaration*/";

        private const string CodeSnippet =
@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Web;
using System.Data.Services;
using System.Data.Services.Common;
using Autofac;
using Rhetos.Logging;
using Module = Autofac.Module;
using DomRepository = Common.DomRepository;

" + InitialCodeGenerator.UsingTag + @"

namespace Services
{
    [System.ComponentModel.Composition.Export(typeof(Module))]
    public class ODataServiceModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ODataService>().InstancePerLifetimeScope();
            base.Load(builder);
        }
    }

    public class ODataServiceHostFactory: Autofac.Integration.Wcf.AutofacServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ODataServiceHost host = new ODataServiceHost(serviceType, baseAddresses);

            return host;
        }
    }

    public class ODataServiceHost:DataServiceHost
    {

        public ODataServiceHost(Type serviceType, Uri[] baseAddresses) : base(serviceType, baseAddresses) 
        {
        }

        protected override void OnOpening ()
        {
            base.OnOpening ();
            Description.Endpoints.Single().EndpointBehaviors.Add(new JSONEndpointBehavior());
            Description.Endpoints.Single().Contract = ContractDescription.GetContract(typeof(IRequestHandler));
            //Description.Endpoints.Single().Binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
            Description.Behaviors.Find<ServiceMetadataBehavior>().HttpGetEnabled = true;
        }
    }

    [System.ComponentModel.Composition.Export(typeof(Rhetos.IService))]
    public class ODataServiceInitializer : Rhetos.IService
    {

        public void Initialize()
        {
            System.Web.Routing.RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute(""OData"", 
                          new ODataServiceHostFactory(), typeof(Services.ODataService)));
        }
    }

    public class JSONMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            HttpRequestMessageProperty requestMessage = request.Properties[""httpRequest""] as HttpRequestMessageProperty;
            if (request.Properties.ContainsKey(""UriTemplateMatchResults""))
            {
                UriTemplateMatch jsonMatch = (UriTemplateMatch)request.Properties[""UriTemplateMatchResults""];
                if (jsonMatch.QueryParameters[""$format""] != null && jsonMatch.QueryParameters[""$format""].ToLower() == ""json"")
                {
                    jsonMatch.QueryParameters.Remove(""$format"");
                    requestMessage.Headers[""Accept""] = ""application/json;odata=light,application/json;odata=verbose"";
                }
            }
            return null;
        }
        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState) { }

    }
    public class JSONEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new JSONMessageInspector());
        }
        public void Validate(ServiceEndpoint endpoint) { }
    }
    public class JSONBehaviorExtensionElement : BehaviorExtensionElement
    {
        public JSONBehaviorExtensionElement() { }
        public override Type BehaviorType
        {
            get { return typeof(JSONEndpointBehavior); }
        }
        protected override object CreateBehavior()
        { return new JSONEndpointBehavior(); }
    }

    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Required)]
    public class ODataService : DataService<RhetosDataSource>
    {
        private readonly Rhetos.Processing.IProcessingEngine _processingEngine;
        private readonly Rhetos.Logging.ILogger _logger;
        private readonly Rhetos.Logging.ILogger _commandsLogger;
        private readonly Rhetos.Logging.ILogger _performanceLogger;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public ODataService(
            Rhetos.Processing.IProcessingEngine processingEngine,
            Rhetos.Logging.ILogProvider logProvider) 
        {
            _processingEngine = processingEngine;
            _logger = logProvider.GetLogger(""ODataService"");
            _commandsLogger = logProvider.GetLogger(""ODataService Commands"");
            _performanceLogger = logProvider.GetLogger(""Performance"");
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();

            ProcessingPipeline.ProcessedRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(ProcessingPipeline_ProcessedRequest);
            ProcessingPipeline.ProcessingRequest += new EventHandler<DataServiceProcessingPipelineEventArgs>(ProcessingPipeline_ProcessingRequest);
        }

        void ProcessingPipeline_ProcessingRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            _commandsLogger.Trace(() => e.OperationContext.AbsoluteRequestUri.PathAndQuery);
            _performanceLogger.Write(_stopwatch, ""ODataService: Started "" + e.OperationContext.AbsoluteRequestUri.PathAndQuery + ""."");
        }

        void ProcessingPipeline_ProcessedRequest(object sender, DataServiceProcessingPipelineEventArgs e)
        {
            _performanceLogger.Write(_stopwatch, ""ODataService: Executed "" + e.OperationContext.AbsoluteRequestUri.PathAndQuery + ""."");
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            this.CurrentDataSource.Initialize(_processingEngine);
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule(""*"", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }
    }

    public class RhetosDataSource
    {
        private Rhetos.Processing.IProcessingEngine _processingEngine;

        public void Initialize(
            Rhetos.Processing.IProcessingEngine processingEngine) 
        {
            _processingEngine = processingEngine;
        }

        " + InitialCodeGenerator.DataSourceExpositionMembersTag + @"
    }  
}

namespace Rhetos.OData
{
    " + InitialCodeGenerator.NamespaceMembersTag + @"
}

";
        
        private static readonly string _rootPath = AppDomain.CurrentDomain.BaseDirectory;

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            codeBuilder.InsertCode(CodeSnippet);

            // global
            codeBuilder.AddReferencesFromDependency(typeof(Guid));
            codeBuilder.AddReferencesFromDependency(typeof(System.Linq.Enumerable));
            codeBuilder.AddReferencesFromDependency(typeof(System.Configuration.ConfigurationElement));
            codeBuilder.AddReferencesFromDependency(typeof(System.Diagnostics.Stopwatch));

            // registration
            codeBuilder.AddReferencesFromDependency(typeof(System.ComponentModel.Composition.ExportAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(Autofac.Integration.Wcf.AutofacServiceHostFactory));

            // wcf dataservices
            codeBuilder.AddReferencesFromDependency(typeof(System.Data.Services.DataServiceException));
            codeBuilder.AddReferencesFromDependency(typeof(System.Data.Services.Common.DataServiceEntityAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.ServiceContractAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Activation.AspNetCompatibilityRequirementsAttribute));
            codeBuilder.AddReferencesFromDependency(typeof(System.ServiceModel.Web.WebServiceHost));
            codeBuilder.AddReferencesFromDependency(typeof(System.Uri));
            codeBuilder.AddReferencesFromDependency(typeof(System.Web.Routing.RouteTable));
            
            // rhetos
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.IService));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Dom.DefaultConcepts.IEntity));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Logging.ILogger));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Logging.LoggerHelper));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.OData.DefaultCommands.ODataQueryCommand));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Processing.IProcessingEngine));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.Security.IClaim));
            codeBuilder.AddReferencesFromDependency(typeof(Rhetos.UserException));

            codeBuilder.AddReference(Path.Combine(_rootPath, "ServerDom.dll"));
            codeBuilder.AddReference(Path.Combine(_rootPath, "Autofac.dll"));
        }

    }
}
