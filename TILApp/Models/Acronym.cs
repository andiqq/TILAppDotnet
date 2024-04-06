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

        public class Dto
        {
            public int Id { get; set; }
            public string? Short { get; set; }
            public string? Long { get; set; }
            public string? UserId { get; set; }

            public Dto() { }

            public Dto(Acronym acronym)
            {
                Id = acronym.Id;
                Short = acronym.Short;
                Long = acronym.Long;
                UserId = acronym.UserId;
            }

            public List<Dto> List(List<Acronym> acronyms)
            {
                return acronyms.Select(a => new Dto(a)).OrderBy(i => i.Id).ToList();
            }
        }
    }
}

