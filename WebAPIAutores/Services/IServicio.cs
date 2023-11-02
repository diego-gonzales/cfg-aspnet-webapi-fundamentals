namespace WebAPIAutores;

public interface IServicio
{
    void DoHomework();
    Guid GetTransient();
    Guid GetScoped();
    Guid GetSingleton();
}

public class ServicioA : IServicio
{
    private readonly ILogger<ServicioA> logger;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;

    public ServicioA(
        ILogger<ServicioA> logger,
        ServicioTransient servicioTransient,
        ServicioScoped servicioScoped,
        ServicioSingleton servicioSingleton
    )
    {
        this.logger = logger;
        this.servicioTransient = servicioTransient;
        this.servicioScoped = servicioScoped;
        this.servicioSingleton = servicioSingleton;
    }

    public void DoHomework() { }

    public Guid GetTransient()
    {
        return servicioTransient.Guid;
    }

    public Guid GetScoped()
    {
        return servicioScoped.Guid;
    }

    public Guid GetSingleton()
    {
        return servicioSingleton.Guid;
    }
}

public class ServicioB : IServicio
{
    public void DoHomework() { }

    // aquí simplemente implementé las métodos de la interfaz y lo dejé tal cual ya que esta clase no la estoy usando
    public Guid GetScoped()
    {
        throw new NotImplementedException();
    }

    public Guid GetSingleton()
    {
        throw new NotImplementedException();
    }

    public Guid GetTransient()
    {
        throw new NotImplementedException();
    }
}

public class ServicioTransient
{
    public Guid Guid = Guid.NewGuid();
}

public class ServicioScoped
{
    public Guid Guid = Guid.NewGuid();
}

public class ServicioSingleton
{
    public Guid Guid = Guid.NewGuid();
}
