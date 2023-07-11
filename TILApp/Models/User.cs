namespace TILApp.Models
{

    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }

        public ICollection<Acronym>? Acronyms { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }

        public static UserDto convertedFrom(User user)
        {
            return new UserDto() { Id = user.Id, Name = user.Name, UserName = user.UserName };
        }

        public static List<UserDto> convertedFrom(List<User> users)
        {
            return users.Select(a => convertedFrom(a)).OrderBy(i => i.Id).ToList();
        }
    }

}