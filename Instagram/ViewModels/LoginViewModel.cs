using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Укажите Логин или Email")]
    public string? Name { get; set; }
    
    [DataType(DataType.EmailAddress , ErrorMessage = "тут должен быть только email")]
    [Display(Name = "Укажите Логин или Email")]
    public string? Email { get; set; }
    
    [DataType(DataType.Password,ErrorMessage = "Тут должен быть только пароль")]
    [Required(ErrorMessage = "Укажите пароль")]
    [Display(Name = "Введите пароль")]
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}