namespace WebAPIAutores;

public class LogResponseMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<LogResponseMiddleware> logger;

    // 'RequestDelegate' lo construye y arma el framework automáticamente, por lo tanto no necesitamos añadirlo en el 'Startup.cs' como servicio.
    public LogResponseMiddleware(RequestDelegate next, ILogger<LogResponseMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        using (var memoryStream = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = httpContext.Response.Body;
            httpContext.Response.Body = memoryStream;

            await next(httpContext);

            memoryStream.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(memoryStream).ReadToEnd();
            memoryStream.Seek(0, SeekOrigin.Begin);

            await memoryStream.CopyToAsync(cuerpoOriginalRespuesta);

            httpContext.Response.Body = cuerpoOriginalRespuesta;
            logger.LogInformation($"---> {respuesta}");
        }
    }
}

// Ya que vamos a hacer un método de extensión y estos solo pueden ser colocados en clases estáticas, pues crear la clase estática
public static class LogResponseMiddlewareExtensions
{
    // el 'this' es como si le dijera que el método de extensión 'UseLogHttpResponse' que podrá ser llamado en cualquier instancia de 'IApplicationBuilder'
    public static IApplicationBuilder UseLogHttpResponse(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LogResponseMiddleware>();
    }
}

// The 'this' keyword before the 'IApplicationBuilder' app parameter indicates that this is an extension method. Extension methods allow you to "add" methods to existing types without creating a new derived type, recompiling, or otherwise modifying the original type. In this case, UseLogHttpResponse is an extension method that can be called on any instance of 'IApplicationBuilder'.
