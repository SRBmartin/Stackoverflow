using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace StackOverflowService.WebRole.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var hasMarker = apiDescription.ActionDescriptor
                .GetCustomAttributes<SwaggerFileUploadAttribute>()
                .Any();

            if (!hasMarker) return;

            operation.consumes = operation.consumes ?? new List<string>();
            if (!operation.consumes.Contains("multipart/form-data"))
                operation.consumes.Add("multipart/form-data");

            operation.parameters = operation.parameters ?? new List<Parameter>();
            if (!operation.parameters.Any(p => p.name == "file"))
            {
                operation.parameters.Add(new Parameter
                {
                    name = "file",
                    @in = "formData",
                    description = "Image file to upload",
                    required = true,
                    type = "file"
                });
            }
        }
    }
}