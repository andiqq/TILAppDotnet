using Microsoft.AspNetCore.Identity;

namespace TILApp.Models;

public class MyUser : IdentityUser
{
    public string? Name { get; set; }
    
    public ICollection<Acronym>? Acronyms { get; set; }
}