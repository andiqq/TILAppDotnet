public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Acronym>? Acronyms { get; set; }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public static CategoryDto convertedFrom(Category category)
    {
        return new CategoryDto() { Id = category.Id, Name = category.Name };
    }

    public static List<CategoryDto> convertedFrom(List<Category> categories)
    {
        return categories.Select(a => convertedFrom(a)).OrderBy(i => i.Id).ToList();
    }
}




