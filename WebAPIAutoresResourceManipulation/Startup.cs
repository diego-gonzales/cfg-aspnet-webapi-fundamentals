using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAutoresResourceManipulation;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigurationService(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services
            .AddControllers()
            .AddJsonOptions(
                options =>
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
            )
            .AddNewtonsoftJson(); // here is configured 'NewtonsofJson'
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
        );

        // después de instalar el automapper configuramos el servicio para poder usarlo
        services.AddAutoMapper(typeof(Startup));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
