using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPIAutores;

public class MyActionFilter : IActionFilter
{
    private readonly ILogger<MyActionFilter> logger;

    public MyActionFilter(ILogger<MyActionFilter> logger)
    {
        this.logger = logger;
    }

    // se realiza antes de ejecutar la acción
    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("Antes de ejecutar la acción");
    }

    // se realiza después de que la acción ya se ha ejecutado
    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("Después de ejecutar la acción");
    }
}
