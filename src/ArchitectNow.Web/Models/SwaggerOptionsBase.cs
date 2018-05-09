using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NSwag.SwaggerGeneration.Processors;

namespace ArchitectNow.Web.Models
{
    public class SwaggerOptionsBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public readonly string DefaultSwaggerRoute = "/docs/v1/swagger.json";

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public readonly string DefaultSwaggerUiRoute = "/docs";

        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string SwaggerRoute { get; set; } = "/docs/v1/swagger.json";
        public string SwaggerUiRoute { get; set; } = "/docs";
        public IEnumerable<Type> Controllers { get; set; }
        public Assembly ControllerAssembly { get; set; } = Assembly.GetEntryAssembly();
        public IList<IDocumentProcessor> DocumentProcessors { get; set; } = new List<IDocumentProcessor>();

        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
        public IList<IOperationProcessor> OperationProcessors { get; set; } = new List<IOperationProcessor>();
    }
}