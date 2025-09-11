using System;

namespace StackOverflowService.WebRole.Swagger
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SwaggerFileUploadAttribute : Attribute { }
}