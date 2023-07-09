//using static TILApp.Models.AcronymDto;
using System.Collections;
using TILApp.Models;

namespace TILAppTest;

public class AcronymConvertTest
{
    public class AcronymTest : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<Acronym>()
                {
                    new Acronym() { Id = 1, Long = "Oh My God", Short = "OMG", UserId = 1 },
                    new Acronym() { Id = 2, Long = "Thank God it's Friday", Short = "TGIF", UserId = 2 }
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    //private static readonly List<Acronym> acronyms = new List<Acronym>() { new Acronym() { Id = 1, Long = "Oh My God", Short = "OMG", UserId = 1 },
    //                                               new Acronym() { Id = 2, Long = "Thank God it's Friday", Short = "TGIF", UserId = 2 }  };


   
    public bool compare(Acronym acronym)
    {
        AcronymDto dtoResult = AcronymDto.convertedFrom(acronym);

        bool isTrue =
            (
            dtoResult.Id == acronym.Id &&
            dtoResult.Long == acronym.Long &&
            dtoResult.Short == acronym.Short &&
            dtoResult.UserId == acronym.UserId
            );
        return isTrue;
    }

    public bool compare(List<Acronym> acronyms)
    {
        List<AcronymDto> dtosResult = AcronymDto.convertedFrom(acronyms);

        bool isTrue = true;

        for (int i = 0;  i < acronyms.Count(); i++)
        {
            isTrue = (
                acronyms[i].Id == dtosResult[i].Id &&
                acronyms[i].Long == dtosResult[i].Long &&
                acronyms[i].Short == dtosResult[i].Short &&
                acronyms[i].UserId == dtosResult[i].UserId &&
                compare(acronyms[i])
                );
            if (!isTrue) return false;
        }
        return true;
    }

    [Theory]
    [ClassData(typeof(AcronymTest))]
    public void TestAcronymDtoConvert(List<Acronym> acronyms)
    {
        bool isTrue = true;

        foreach (Acronym acronym in acronyms)
        {
            if (!compare(acronym)) isTrue = false;
            break;
        }

        Assert.True(isTrue, "conversion error");
    }

    [Theory]
    [ClassData(typeof(AcronymTest))]
    public void TestAcronymsDtoConvert(List<Acronym> acronyms)
    {
        var isTrue = compare(acronyms);
        Assert.True(isTrue, "List conversion error");
    }
}
