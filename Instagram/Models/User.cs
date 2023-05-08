using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Models;

public class User : IdentityUser
{
    [NotMapped]
    public IFormFile Avatar { get; set; }
    public string PathFile { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Male { get; set; }
    public List<Post> Posts { get; set; }

}