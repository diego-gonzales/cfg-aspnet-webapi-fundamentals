using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace WebAPIAutoresAvanzados;

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

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo()
                {
                    Title = "Web Api Autores",
                    Version = "v1",
                    Description = "Esta es una web api para trabajar con libros y autores",
                    Contact = new OpenApiContact
                    {
                        Name = "Diego Gonzales",
                        Email = "diego@email.com",
                        Url = new Uri("https://github.com/diego-gonzales")
                    },
                    License = new OpenApiLicense { Name = "MIT" }
                }
            );

            options.SwaggerDoc(
                "v2",
                new OpenApiInfo() { Title = "Web Api Autores", Version = "v2" }
            );

            // Configuración de swagger para agregar el parámetro 'includeHATEOAS' a todos los endpoints
            options.OperationFilter<AddHATEOASParameter>();

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                }
            );

            // Configuración de swagger para agregar los comentarios de los métodos
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        services
            .AddControllers(options =>
            {
                options.Conventions.Add(new SwaggerGroupByVersion());
            })
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

        services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdmin", policy => policy.RequireClaim("isAdmin"));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins("")
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "totalItems" });
            });
        });

        services.AddDataProtection();

        services.AddTransient<LinksGenerator>();
        services.AddTransient<HATEOASAuthorFilterAttribute>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // Configuración de 'Application Insights'
        services.AddApplicationInsightsTelemetry(actions =>
        {
            actions.ConnectionString = Configuration["ApplicationInsights:ConnectionString"];
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API Autores v1");
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "Web API Autores v2");
        });

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
