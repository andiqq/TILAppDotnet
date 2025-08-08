namespace TILApp.Models;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Acronym>? Acronyms { get; set; }

    public CategoryDto ToDto() => new() { Id = Id, Name = Name };
}

public class CategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}