# .NET core from Soup-to-Nuts.

Build a API backend for your application using the newest version of .NET that can be developed anywhere and run anywhere else.

## Environment Setup

Download and Install the .Net Core SDK from [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core). You will also need a text editor you're comfortable with. 

[Postman](https://www.getpostman.com) is optional but useful for creating web service calls using JWT tokens.


## Hello World

Let's start with a simple "Hello World" application to just test that our environment is working correctly. Open a terminal and navigate to an empty directory. From here run:
```bash
dotnet new
```
This command creates a project.json file and a Program.cs file. There is nothing special about this process and these files could have been created by hand. The Program.cs file contains the main entry point of the application. The project.json file replaces the .sln and .csproj files of traditional .net with a simpler file that looks much like an [NPM package.json](https://docs.npmjs.com/files/package.json). Node.js developers should be very familiar with the syntax. The full file reference can be found at [https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json).

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
"Microsoft.AspNetCore.Server.Kestrel": "1.0.1",
"Microsoft.Extensions.Configuration.EnvironmentVariables": "1.0.0"
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

