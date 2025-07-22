using Microsoft.AspNetCore.Identity;

namespace TILApp.Models;

public class User : IdentityUser
{
    public string? Name { get; set; } = String.Empty;

    public ICollection<Acronym>? Acronyms { get; set; }

    public class Public
    {
     //   private string? Id { get; set; }
        public string? Name { get; set; } = String.Empty;
        public string? UserName { get; set; } = String.Empty;

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
