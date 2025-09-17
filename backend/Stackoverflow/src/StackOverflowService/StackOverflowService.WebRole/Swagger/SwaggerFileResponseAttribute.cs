using System;

namespace StackOverflowService.WebRole.Swagger
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class SwaggerFileResponseAttribute : Attribute
	{
        public string[] ContentTypes { get; }
        public SwaggerFileResponseAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes ?? Array.Empty<string>();
        }
    }
}