
    public class Acronym
    {
        public int Id { get; set; }
        public string? Short { get; set; }
        public string? Long { get; set; }

        public User? User { get; set; }
        public int? UserId { get; set; }

        public ICollection<Category>? Categories { get; set; }
    }

    public class AcronymDto
    {
        public int Id { get; set; }
        public string? Short { get; set; }
        public string? Long { get; set; }
        public int? UserId { get; set; }

        public static AcronymDto convertedFrom(Acronym acronym)
        {
            return new AcronymDto() { Id = acronym.Id, Long = acronym.Long, Short = acronym.Short, UserId = acronym.UserId };
        }

        public static List<AcronymDto> convertedFrom(List<Acronym> acronyms)
        {
            return acronyms.Select(a => convertedFrom(a)).OrderBy(i => i.Id).ToList();
        }
    }
