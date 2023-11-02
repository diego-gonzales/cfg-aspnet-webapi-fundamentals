using WebAPIAutores;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
startup.ConfigureServices(builder.Services);

var app = builder.Build();

var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));

// Configure the HTTP request pipeline.
startup.Configure(app, app.Environment, servicioLogger);

app.Run();
