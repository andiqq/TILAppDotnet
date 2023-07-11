using System.Collections;
using TILApp.Models;
using TILApp.Controllers;

namespace TILAppTest;

public class CategoryConvertTest
{
    public class CategoryTest : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<Category>()
                {
                    new Category() { Id = 1, Name = "Classic" },
                    new Category() { Id = 2, Name = "Cool" }
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    public bool compare(Category category, CategoryDto categoryDto)
    {
        bool isTrue =
            (
            categoryDto.Id == category.Id &&
            categoryDto.Name == category.Name  
            );
        return isTrue;
    }

    public bool compare(List<Category> categories, List<CategoryDto> dtos)
    {
        bool isTrue = true;

        for (int i = 0;  i < categories.Count(); i++)
        {
            isTrue = (compare(categories[i], dtos[i]));
            if (!isTrue) return false;
        }
        return true;
    }

    [Theory]
    [ClassData(typeof(CategoryTest))]
    public void TestCategoryDtoConvert(List<Category> categories)
    {
        bool isTrue = true;

        foreach (Category category in categories)
        {
            CategoryDto dtoResult = CategoryDto.convertedFrom(category);
            if (!compare(category, dtoResult)) isTrue = false;
            break;
        }

        Assert.True(isTrue, "category conversion error");
    }

    [Theory]
    [ClassData(typeof(CategoryTest))]
    public void TestCategoryssDtoConvert(List<Category> categories)
    {
        List<CategoryDto> dtos = CategoryDto.convertedFrom(categories);
        var isTrue = compare(categories, dtos);
        Assert.True(isTrue, "categoryList conversion error");
    }

   
}
