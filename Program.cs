using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace PokemonService
{
    /*
    Main entry point for the application. Starts the Kestrel server
    */
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
        }
    }
}