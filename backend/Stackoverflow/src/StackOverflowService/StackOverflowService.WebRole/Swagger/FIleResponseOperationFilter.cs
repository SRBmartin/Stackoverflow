using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace StackOverflowService.WebRole.Swagger
{
	public class FIleResponseOperationFilter : IOperationFilter
	{
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var attr = apiDescription.GetControllerAndActionAttributes<SwaggerFileResponseAttribute>().FirstOrDefault();
            if (attr == null) return;

            if (operation.produces == null) operation.produces = new List<string>();
            operation.produces.Clear();

            var contentTypes = (attr.ContentTypes != null && attr.ContentTypes.Length > 0)
                ? attr.ContentTypes
                : new[] { "application/octet-stream" };

            foreach (var ct in contentTypes)
                operation.produces.Add(ct);

            if (operation.responses != null && operation.responses.ContainsKey("200"))
            {
                operation.responses["200"].schema = new Schema { type = "file" };
                if (string.IsNullOrEmpty(operation.responses["200"].description))
                    operation.responses["200"].description = "Binary file";
            }
        }
    }
}