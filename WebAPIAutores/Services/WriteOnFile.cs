namespace WebAPIAutores;

public class WriteOnFile : IHostedService
{
    private readonly IWebHostEnvironment env;
    private readonly string fileName = "MyFile.txt";
    private Timer timer;

    public WriteOnFile(IWebHostEnvironment env)
    {
        this.env = env;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        WriteMessage("Process initialized");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer.Dispose();
        WriteMessage("Process finished");
        return Task.CompletedTask;
    }

    private void WriteMessage(string message)
    {
        var path = $@"{env.ContentRootPath}\wwwroot\{fileName}";

        using (StreamWriter writer = new StreamWriter(path, append: true))
        {
            writer.WriteLine(message);
        }
    }

    private void DoWork(object state)
    {
        WriteMessage("Process running: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
    }
}
