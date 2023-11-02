using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores;

public class Startup_Old
{
    public Startup_Old(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthorization();
        services
            .AddControllers()
            .AddJsonOptions(
                x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
        );

        // probando servicios
        services.AddTransient<IServicio, ServicioA>();

        services.AddTransient<ServicioTransient>();
        services.AddScoped<ServicioScoped>();
        services.AddSingleton<ServicioSingleton>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        // 'Use' se usa cuando quiero 'agregar' mi propio proceso a la tubería de procesos y permitir que los demás procesos se sigan ejecutando.
        app.Use(
            async (httpContext, next) =>
            {
                // Entrada del middleware

                // 1. Creo un MemoryStream para poder manipular
                // y copiarme el cuerpo de la respuesta.
                // Esto se hace porque el stream del cuerpo de la
                // respuesta no tiene permisos de lectura.
                using (var memoryStream = new MemoryStream())
                {
                    // 2. Guardo la referencia del Stream donde se
                    // escribe el cuerpo de la respuesta
                    var cuerpoOriginalRespuesta = httpContext.Response.Body;

                    // 3. Cambio el stream por defecto del cuerpo
                    // de la respuesta por el MemoryStream creado
                    // para poder manipularlo
                    httpContext.Response.Body = memoryStream;

                    // 4. Esperamos a que el siguiente middleware
                    // devuelva la respuesta.
                    await next.Invoke();

                    // Salida del middleware

                    // 5. Nos movemos al principio del MemoryStream
                    // Para copiar el cuerpo de la respuesta
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // 6. Leemos stream hasta el final y almacenamos
                    // el cuerpo de la respuesta obtenida
                    string respuesta = new StreamReader(memoryStream).ReadToEnd();

                    // 5. Nos volvemos a posicionar al principio
                    // del MemoryStream para poder copiarlo al
                    // cuerpo original de la respuesta
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // 7. Copiamos el contenido del MemoryStream al
                    // stream original del cuerpo de la respuesta
                    await memoryStream.CopyToAsync(cuerpoOriginalRespuesta);

                    // 8.Volvemos asignar el stream original al el cuerpo
                    // de la respuesta para que siga el flujo normal.
                    httpContext.Response.Body = cuerpoOriginalRespuesta;

                    // 9. Escribimos en el log la respuesta obtenida
                    logger.LogInformation($"---> {respuesta}");
                }
            }
        );

        app.Map(
            "/ruta1",
            appcita =>
            {
                // 'Run' se usa cuando queremos 'interceptar' la tubería de procesos
                appcita.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería");
                });
            }
        );

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
