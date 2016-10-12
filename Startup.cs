using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace PokemonService
{
    /*
    This class configures ASP.Net
    */
    public class Startup
    {
        /*
        Wire the Dependency injection container by adding services to it.
        */
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<PokemonDbContext>(options =>
            {
                options.UseSqlite("Filename=pokemon.db");
            });

        }

        /*
        Configure the request pipeline 
        */
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);

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

            app.UseCors(policyBuilder =>
            {
                policyBuilder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            });

            app.UseMvc();
        }
    }
}