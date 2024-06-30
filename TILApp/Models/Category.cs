namespace TILApp.Models;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Acronym>? Acronyms { get; set; }

    public CategoryDto toDto()
    {
        CategoryDto dto = new();
        dto.Id = Id;
        dto.Name = Name;
        return dto;
    }
    
}

public class CategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}