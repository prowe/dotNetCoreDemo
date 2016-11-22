using Microsoft.EntityFrameworkCore;
using System;

namespace PokemonService
{
    /*
    This configures the database for Entity Framework Core
    and is the entry point for databse operations.
    */
    public class PokemonDbContext : DbContext
    {
        public DbSet<Pokemon> Pokemon { get; set; }

        public PokemonDbContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            if(connectionString == null)
            {
                connectionString = "Filename=pokemon.db";
            }
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}