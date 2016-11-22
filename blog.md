# .NET core from Soup-to-Nuts.

Build a API backend for your application using the newest version of .NET that can be developed anywhere and run everywhere.

## Environment Setup

Download and Install the .Net Core SDK from [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core). You will also need a text editor you're comfortable with. 

[Postman](https://www.getpostman.com) is optional but useful for creating web service calls using JWT tokens.


## Hello World

Let's start with a simple "Hello World" application to just test that our environment is working correctly. Open a terminal and navigate to an empty directory. From here run:
```bash
dotnet new
```
This command creates a project.json file and a Program.cs file. There is nothing special about this process and these files could have been created by hand. The Program.cs file contains the main entry point of the application. The project.json file replaces the .sln and .csproj files of traditional .net with a simpler file that looks much like an [NPM package.json](https://docs.npmjs.com/files/package.json). Node.js developers should be very familiar with the syntax. The full reference can be found at [https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json).

Run the project by executing:
```bash
dotnet restore
dotnet run
```
This will download referenced packages from [Nuget](https://www.nuget.org) and then display the "Hello World" message.

## Simple Server 

Since we are building a RESTful web service, we will need something to listen to http requests. Traditionally in the .net world this was fulfilled by IIS. Here we are going to use an embedded application server called Kestrel. This is the same strategy employed by [Spring Boot](https://spring.io/guides/gs/rest-service/) in the Java space.

Add the following libraries to the dependencies section of the project.json file (versions may differ)
```json
"Microsoft.AspNetCore.Server.Kestrel": "1.1.0",
"Microsoft.Extensions.Configuration.EnvironmentVariables": "1.1.0"
```

To start the server with the application, remove the Console.out call inside the Program.cs main method and replace it with the following:
```C#
var webHost = new WebHostBuilder()
    .UseKestrel()
    .UseStartup<Program>()
    .Build();
webHost.Run();
```
You will also need the following usings:
```C#
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
```

Kestrel needs a "Startup" class to be handed to it. This class contains configuration callback methods that handle everything from the dependency injection container to routing rules.

Add the following method to the Program class:
```
public void ConfigureServices(IServiceCollection services)
{
}
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
}
```

The first method (ConfigureServices) is where "services" (Java folks, read as "Beans") can be added to the dependency injection container.

The Configure method will be autowired with any services declared in its parameter list and is responsible for configuring the Kestrel request pipline. Here is were we will enable security and CORS, etc.

Run the application as before and you should see this message:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```
Issue any request to localhost on that port and you should recieve a 404 back.

## Adding MVC

So far we have a simple HTTP server, but are lacking controller routing. Start by adding the following dependencies to your project.json:
```json
"Microsoft.AspNetCore.Mvc": "1.1.0",
"Microsoft.AspNetCore.Routing": "1.1.0",
"Microsoft.Extensions.Logging": "1.1.0",
"Microsoft.Extensions.Logging.Console": "1.1.0",
"Microsoft.Extensions.Logging.Debug": "1.1.0"
```

Then in Program.cs add this line to the ConfigureServices method to add the MVC related services to the DI container:
```C#
services.AddMvc();
```
In the Configure method add these lines to enable console logging, which is very useful for development, and enable MVC routing:
```C#
loggerFactory.AddConsole(LogLevel.Information);        
app.UseMvc();
```

We need to create a Model class to represent our domain entities. Since we are modeling a "Pokemon Service", create a Pokemon.cs with the following content:
```c#
using System.ComponentModel.DataAnnotations;

namespace ConsoleApplication
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
```

Now let's add a simple controller that returns a hard coded Pokemon for every get request to just test that we have routing setup properly. Create a PokemonController with this content:
```c#
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsoleApplication
{
    [Route("pokemon")]
    public class PokemonController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var pokemon = new Pokemon {
                Id = 1,
                Name = "Pikachu",
                Type = "Electric"
            };
            return Ok(pokemon);
        }
    }
}
```

Notice that we are extending "Controller" and not "ApiController" as we would in Web API 2. 
ASP.net core has streamlined the process for handling web service calls with traditional HTML requests. 
Also notice that nowhere do we specify that we are returning JSON or marshalling the object to JSON.

Start the app server and hit the "pokemon endpoint" with curl and you should see the following:
```bash
$ curl http://localhost:5000/pokemon/3
{"id":1,"name":"Pikachu","type":"Electric"}
```

We now have a working web service.

## Adding peristance to our API

[Entity Framework Core](https://docs.microsoft.com/en-us/ef/) (formerly known as Entity Framework 7) is a complete rewrite of Entity Framework. This provides the opportunity to shed much of the dead weight that EF has picked up over the years and has made it a much faster and easier to use system. If you have shyed away from EF in the past as too heavyweight and inefficient, I suggest you take a look at EF Core.

Start with adding the following to the dependencies section of your project.json:
```json
"Microsoft.EntityFrameworkCore.Sqlite": "1.1.0",
"Microsoft.EntityFrameworkCore.Design": {
    "version": "1.1.0",
    "type": "build"
}
```

We are going to be using Sqlite for this project because it is portable and easy to setup. To use SQL server (or Postgres etc,) include the appropriate dependency.

To use database migrations add a new section to the project.json as a sibling to the dependencies section:
```json
"tools": {
    "Microsoft.EntityFrameworkCore.Tools.DotNet": "1.1.0-preview4-final"
}
```
"Tools" in .Net core allow new commands to be added to the dotnet command for development or build time tasks. 
This tool gives us the "dotnet ef" command.

Just like previous versions of Entity Framework, we need to create a "DbContext" class to act as an access point for data operations. Instances of this class are not threadsafe and should not be shared accross the application, but should be shared accross the request. (For those of you from the Hibernate space, this is the equivilent of a "Session" NOT a "SessionFactory"). Luckly, the DI container can handle all of this scoping for us.

```C#
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication
{
    public class PokemonDbContext : DbContext
    {
        public DbSet<Pokemon> Pokemon {get; set;}

        public PokemonDbContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=pokemon.db");
        }
    }
}
```

Just like the MVC component, we need to add the Entity Framework services to the DI container. Add these lines to Program.cs:
```c#
using Microsoft.EntityFrameworkCore;
...
public void ConfigureServices(IServiceCollection services)
{
...
    services.AddDbContext<PokemonDbContext>();
}
```

The "AddDbContext" extension method registers various services with the DI container and sets up our PokemonDbContext class to be created once per request and available for dependency injection. We are using Sqllite here so I am specifying the path to the database file. For SqlServer you would specify a connection string.

Now we need to access our DBContext inside our controller. We can autowire any registered service into the constructor of the controller. Do this by adding this field and constructor to PokemonController.cs:
```C#
private readonly PokemonDbContext dbContext;
public PokemonController(PokemonDbContext dbContext)
{
    this.dbContext = dbContext;
}
```

Controllers in ASP.net are created once per request so it is safe to hold an instance reference to it. This is an important distinction to make when comparing the way ASP.net handles requests to other MVC frameworks (i.e. Spring MVC).

Now we can update our Get method as follows:
```C#
[HttpGet("{id}")]
public IActionResult Get(int id)
{
    var pokemon = dbContext.Pokemon.Find(id);
    if (pokemon == null)
    {
        return NotFound();
    }
    return Ok(pokemon);
}
```

Let's add a listing method:
```C#
[HttpGet]
public IEnumerable<Pokemon> Get()
{
    return dbContext.Pokemon.ToList();
}
```

Now If we start the app and curl the /pokemon end point (the Get method that takes no parameter), we will get a 500 back because there is no table called "Pokemon" in the datbase.
This is because we have not yet created the schema. We can use Entity Framework to create tables by creating some "migrations". From the command line run the following two commands:

```bash
$ dotnet ef migrations add createPokemon
$ dotnet ef database update
```

The first command creates a "Migration" which is a file in the migrations folder that will perform the needed DML to get the databse to the target structure EF expects. The second command upgrades the configured database. A cool feature of the EF toolset is that it actually looks at the application to determine the configuration for connecting to the database. This is why we didn't have to specify any configuration as part of the "database update" command.

If we rerun the application and hit (http://localhost:5000/pokemon/) we should now get an empty Json array.

To support posting a new Pokemon, add this method to our PokemonController:
```C#
[HttpPost]
public IActionResult Post([FromBody]Pokemon newPokemon)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    dbContext.Pokemon.Add(newPokemon);
    dbContext.SaveChanges();
    return Ok(newPokemon);
}
```

The ModelState check at the beginning of the method determines if validation has passed. 
We can add validation rules by adding any attribute that inherents from [System.ComponentModel.DataAnnotations.ValidationAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.validationattribute(v=vs.110).aspx) to a property on our model.

We add the Pokemon to the "Set" abstraction that the DbContext provides. Then call "SaveChanges" to flush the changes to the DB. This call will start a transaction, flush all pending changes to the DB and commit it. We should be able to see the SQL executed in the console due to our logging setup from earlier.

To update a Pokemon add this method:
```C#
[HttpPut("{id}")]
public IActionResult Put(int id, [FromBody]Pokemon pokemon)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    pokemon.Id = id;
    dbContext.Pokemon.Update(pokemon);
    dbContext.SaveChanges();
    return Ok(pokemon);
}
```

Deletes can be handled as follows:
```c#
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    var pokemon = dbContext.Pokemon.Find(id);
    if(pokemon != null)
    {
        dbContext.Pokemon.Remove(pokemon);
        dbContext.SaveChanges();
    }
    return Ok();
}
```

## Adding Security

Let's secure our service using OpenID Connect JWT Bearer Tokens. This will allow our service to be called by both end users as well as other systems.

Add a new dependency to project.json:
```json
"Microsoft.AspNetCore.Authentication.JwtBearer": "1.1.0"
```

Mark the controller as requiring authorization by adding this attribute:
```c#
[Authorize]
```

In the Configure method of Program.cs add the JWT bearer token configuration:
```c#
app.UseJwtBearerAuthentication(new JwtBearerOptions
{
    AutomaticAuthenticate = true,
    MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration",
    TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidIssuer = "accounts.google.com"
    }
});
```

This configuration will authenticate any incoming request to Google and allow any google user to authenticate the application. 
We can use any OpenID Connect provider by changing the metadata address.
The "audience" is our application, so "ValidateAudience" determines if we should validate that the token was issued specifically for us.

Start the application and attempt to request http://localhost:5000/pokemon and be greeted with a 401 response. 

To invoke services secured by JWT I like to use [Postman](https://www.getpostman.com) because it has built in support for fetching tokens from an OAuth provider.
Instructions on using Postman to generate OAuth tokens can be found at https://www.getpostman.com/docs/helpers#oauth-20. You will need to set up an "Application" inside your google account to generate a client ID/Secret for Postman to use.

## Conclusion

At this point we have a canonical example of a modern RESTful web service that:
 * Is easy to deploy
 * Is fully cross platform
 * Is secured based on an industry standard that has wide support
 * Persists its data to a database
 * Uses dependency injection
 * Has no "boilerplate" or other overhead



