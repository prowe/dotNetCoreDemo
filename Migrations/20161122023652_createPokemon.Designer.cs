using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PokemonService;

namespace work.Migrations
{
    [DbContext(typeof(PokemonDbContext))]
    [Migration("20161122023652_createPokemon")]
    partial class createPokemon
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("PokemonService.Pokemon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Pokemon");
                });
        }
    }
}
