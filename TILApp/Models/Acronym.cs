namespace TILApp.Models
{

    public class Acronym
    {
        public int Id { get; init; }
        public string? Short { get; set; }
        public string? Long { get; set; }

        public User? User { get; set; } 
        public string? UserId { get; set; }

        public ICollection<Category>? Categories { get; set; }

        public AcronymDto ToDto() => new() { Long = Long, Short = Short, UserId = UserId };
        
    }

    public class AcronymDto
    {
        public string? Short { get; init; }
        public string? Long { get; init; }
        public string? UserId { get; init; }
    }
}

