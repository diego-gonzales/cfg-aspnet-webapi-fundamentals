using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutoresAvanzados;

// Clase que me permite agrupar los endpoints de swagger por versión, es decir, en la documentación de swagger me va a mostrar los endpoints de la versión 1 y los endpoints de la versión 2. Nota: Esta clase la tengo que registrar en el método 'ConfigureServices' de la clase 'Startup.cs', dentro de la sección 'services.AddControllers(options => options.Conventions.Add(new SwaggerGroupByVersion()));'
public class SwaggerGroupByVersion : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var controllerNamespace = controller.ControllerType.Namespace; // Controllers.v1
        var apiVersion = controllerNamespace.Split('.').Last().ToLower(); // v1

        controller.ApiExplorer.GroupName = apiVersion;
    }
}
