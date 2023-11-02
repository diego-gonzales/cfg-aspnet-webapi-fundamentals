using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores;

public class StartupOld2
{
    public StartupOld2(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services
            .AddControllers(options =>
            {
                // In this way we add a global custom filter
                options.Filters.Add(typeof(MyExceptionFilter));
            })
            .AddJsonOptions(
                x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            );

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
        );

        services.AddResponseCaching();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Servicio personalizado para escribir código recurrente, en este caso escribir en un TXT cada 5s.
        services.AddHostedService<WriteOnFile>();

        // we have to register in the DI system our custom filter
        services.AddTransient<MyActionFilter>();

        // probando servicios
        services.AddTransient<IServicio, ServicioA>();

        services.AddTransient<ServicioTransient>(); // Transient siempre me da una nueva instancia de una clase, aunque estemos en el mismo contexto HTTP
        services.AddScoped<ServicioScoped>(); // Scoped: te da la misma instancia de una clase en el mismo contexto HTTP.
        services.AddSingleton<ServicioSingleton>(); // Singleton: siempre te da la misma instancia de una clase incluso en distintas peticiones HTTP, independientemente de que sean usuarios distintos los que esten haciendo esas peticiones.
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        // app.UseMiddleware<LogResponseMiddleware>(); // esta es la primera forma de llamar un middleware
        app.UseLogHttpResponse(); // esta es la otra manera, creando un método de extensión.

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

        // para poder usar filtros
        app.UseResponseCaching();
        app.UseAuthorization();

        // debe ir al último ya que aquí es donde mapeo los controladores
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
