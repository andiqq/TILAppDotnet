using Microsoft.AspNetCore.Identity;

namespace TILApp.Models;

public class User : IdentityUser
{
    public string? Name { get; set; } = string.Empty;

    public ICollection<Acronym>? Acronyms { get; set; }

    public class Public
    {
        public string? Name { get; } = string.Empty;

        public string? UserName { get; } = string.Empty;

        public Public() { }

        public Public(User user)
        {
            Name = user.Name;
            UserName = user.UserName;
        }

        public List<Public> List(List<User> users)
        {
            return users.Select(a => new Public(a)).OrderBy(i => i.UserName).ToList();
        }
    }
}
