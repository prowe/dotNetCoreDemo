using System.ComponentModel.DataAnnotations;

namespace PokemonService
{
    public class Pokemon
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
    }
}