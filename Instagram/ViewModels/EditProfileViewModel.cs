using Instagram.Models;

namespace Instagram.ViewModels;

public class EditProfileViewModel
{
    public string Id { get; set; }
    public IFormFile? Avatar { get; set; }
    
    public string? PathFile { get; set; }

    public string? Name { get; set; }

    public string? About { get; set; }

    public string? Male { get; set; }
    
    public string? Phone { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Login { get; set; }
    
    public string ConfirmPassword { get; set; }
}