
### init Project
```bash
dotnet new
```

```C#
var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddEnvironmentVariables(prefix: "ASPNETCORE_")
    .Build();

var host = new WebHostBuilder()
    .UseConfiguration(config)
    .UseKestrel()
    .UseStartup<Startup>()
    .Build();

host.Run();

public class Startup
{
    public IConfigurationRoot Configuration { get; }
    public Startup(IHostingEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole(LogLevel.Information);
        app.UseMvc();
    }
}

public class Pokemon
{
    public string Name { get; set; }
}

[Route("pokemon")]
public class PokemonController : Controller
{
    [HttpGet]
    public IEnumerable<Pokemon> Get()
    {
        return new List<Pokemon> {
            new Pokemon { Name = "Pikachu" }
        };
    }
}
```
### add ef

### add cors

### update pokedex

### add authentication
