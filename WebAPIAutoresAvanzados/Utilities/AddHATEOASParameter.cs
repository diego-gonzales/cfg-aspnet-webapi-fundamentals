using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPIAutoresAvanzados;

// Clase que se usa para agregar un parámetro a todos los endpoints de swagger (en este caso el parámetro 'includeHATEOAS'). lo agregamos en Startup.cs, dentro de ConfigureServices, en la configuración de swagger.
public class AddHATEOASParameter : IOperationFilter // interface de swagger
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // we make this validation to avoid adding the parameter to endpoints that are not GET
        if (context.ApiDescription.HttpMethod != "GET")
        {
            return;
        }

        // if (operation.Parameters == null)
        // {
        //     operation.Parameters = new List<OpenApiParameter>();
        // }

        // compounding assignment ??= (null-coalescing assignment) operator, es equivalente a lo anterior.
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "includeHATEOAS",
                In = ParameterLocation.Header,
                Required = false
            }
        );
    }
}
