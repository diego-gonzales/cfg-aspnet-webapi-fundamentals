using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WebAPIAutoresSeguridad;

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
            .AddNewtonsoftJson();
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
        );
        services.AddAutoMapper(typeof(Startup));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // Realizamos las configuraciones para hacer que se valide de manera correcta nuestro JWT, aquí le indicamos que firma tiene que usar para validar el token, si tiene que validar el tiempo de vida del token, entre otras cosas.
            .AddJwtBearer(
                options =>
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JWTSecret"])
                        ),
                        ClockSkew = TimeSpan.Zero
                    }
            );

        // Configuración de Identity
        services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
