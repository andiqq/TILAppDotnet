using System.Collections;
using Moq;
using TILApp.Models;
using TILApp.Controllers;

namespace TILAppTest;

public class UserConvertTest
{
    public class UserTest : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new List<User>()
                {
                    new User() { Id = 1, Name = "Egon Olsen", UserName = "eolsen" },
                    new User() { Id = 2, Name = "Peter Parker", UserName = "spman" }
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    public bool compare(User user, UserDto userDto)
    {
        bool isTrue =
            (
            userDto.Id == user.Id &&
            userDto.Name == user.Name &&
            userDto.UserName == user.UserName 
            );
        return isTrue;
    }

    public bool compare(List<User> users, List<UserDto> dtos)
    {
        bool isTrue = true;

        for (int i = 0;  i < users.Count(); i++)
        {
            isTrue = (compare(users[i], dtos[i]));
            if (!isTrue) return false;
        }
        return true;
    }

    [Theory]
    [ClassData(typeof(UserTest))]
    public void TestUserDtoConvert(List<User> users)
    {
        bool isTrue = true;

        foreach (User user in users)
        {
            UserDto dtoResult = UserDto.convertedFrom(user);
            if (!compare(user, dtoResult)) isTrue = false;
            break;
        }

        Assert.True(isTrue, "user conversion error");
    }

    [Theory]
    [ClassData(typeof(UserTest))]
    public void TestUserssDtoConvert(List<User> users)
    {
        List<UserDto> dtos = UserDto.convertedFrom(users);
        var isTrue = compare(users, dtos);
        Assert.True(isTrue, "userList conversion error");
    }

    private IEnumerable<User> GetFakeData()
    {
        return new List<User>()
                {
                    new User() { Id = 1, Name = "Egon Olsen", UserName = "eolsen" },
                    new User() { Id = 2, Name = "Peter Parker", UserName = "spman" }
                };
    }

    [Fact]
    public void GetUsersTest()
    {
        var service = new Mock<AcronymContext>();

        var users = GetFakeData();

        service.Setup(x => x.User.AddRange(users));

        var controller = new UserController(service.Object);

        var results = controller.GetUser();

    }

}
