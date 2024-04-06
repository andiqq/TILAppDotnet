namespace TILApp.Models;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Acronym>? Acronyms { get; set; }

    public class CDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public CDto() { }

        public CDto(Category category)
        {
            Id = category.Id;
            Name = category.Name;
        }

        public List<CDto> List(List<Category> categories)
        {
            return categories.Select(a => new CDto(a)).OrderBy(i => i.Id).ToList();
        }
    }
}