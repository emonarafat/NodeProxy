
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NodeProxy.Extensions
{
    public class SwaggerExcludeFilter : IDocumentFilter
    {
        #region ISchemaFilter Members
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var schema in context.SchemaRepository.Schemas.Where(item => item.Key.Contains("WithData")))
            {
                context.SchemaRepository.Schemas.Remove(schema.Key);
            }
        }
        #endregion

    }
    public class AuthHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "SoapAction",
                In = ParameterLocation.Header,
                Required = true,
                Style = ParameterStyle.Form
            });
        
        operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
              Required=true,
              Style =ParameterStyle.Form
            });
        }
    }
}
