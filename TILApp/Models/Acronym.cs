namespace TILApp.Models
{

    public class Acronym
    {
        public int Id { get; set; }
        public string? Short { get; set; }
        public string? Long { get; set; }

        public User? User { get; set; } 
        public string? UserId { get; set; }

        public ICollection<Category>? Categories { get; set; }

        public AcronymDto ToDto() => new() { Id = Id, Long = Long, Short = Short, UserId = UserId };
        
    }

    public class AcronymDto
    {
        public int Id { get; set; }
        public string? Short { get; set; }
        public string? Long { get; set; }
        public string? UserId { get; set; }
    }
}

