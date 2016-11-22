using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PokemonService
{
    [Route("pokemon")]
    [Authorize]
    public class PokemonController : Controller
    {
        private readonly PokemonDbContext dbContext;

        //ASP.Net will autowire the constructer from the configured "Services"
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
            var pokemon = dbContext.Pokemon.Find(id);
            if (pokemon == null)
            {
                return NotFound();
            }
            return Ok(pokemon);
        }

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
    }
}