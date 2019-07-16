using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PersonalSiteApi.StartupHelper {
	public class BasePathFilter : IDocumentFilter
	{
		public void Apply(
			OpenApiDocument swaggerDoc,
			DocumentFilterContext context
		)
		{
			swaggerDoc.Servers = new List<OpenApiServer>() { new OpenApiServer { Url = "/api" } };
		}
	}
}