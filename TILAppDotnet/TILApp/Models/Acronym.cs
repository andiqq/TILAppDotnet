using System.ComponentModel.DataAnnotations.Schema;

namespace TILApp.Models
{

    public class Acronym
    {
        public int Id { get; init; }
        public string? Short { get; set; } = string.Empty;
        public string? Long { get; set; } = string.Empty;

        public User? User { get; set; } 
        
        public required string? UserId { get; set; }

        public ICollection<Category>? Categories { get; set; }

        public AcronymDto ToDto() => new() {Id = Id, Long = Long, Short = Short, UserId = UserId };
        
    }

    public class AcronymDto
    {
        public int Id { get; init; }
        public string? Short { get; init; } = string.Empty;
        public string? Long { get; init; }
        public string? UserId { get; init; } = string.Empty;
    }
}

