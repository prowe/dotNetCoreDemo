using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Cors;

using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace PokemonService
{
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

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<PokemonDbContext>(options =>
            {
                options.UseSqlite("Filename=pokemon.db");
            });

        }
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

    public class Pokemon
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
    }

    [Route("pokemon")]
    public class PokemonController : Controller
    {
        private readonly PokemonDbContext dbContext;

        public PokemonController(PokemonDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Pokemon> Get()
        {
            return dbContext.Pokemon.ToList();
        }

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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var pokemon = dbContext.Pokemon
                .FirstOrDefault(p => p.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }
            return Ok(pokemon);
        }

        [HttpPutAttribute("{id}")]
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

        [HttpDeleteAttribute("{id}")]
        public IActionResult Delete(int id)
        {
            dbContext.Pokemon.RemoveRange(
                dbContext.Pokemon.Where(p => p.Id == id).ToArray()
            );
            dbContext.SaveChanges();
            return Ok();
        }
    }

    public class PokemonDbContext : DbContext
    {
        public DbSet<Pokemon> Pokemon { get; set; }

        public PokemonDbContext(DbContextOptions options) : base(options)
        { }
    }
}