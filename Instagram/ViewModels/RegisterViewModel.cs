using System.ComponentModel.DataAnnotations;
using Instagram.Attributes;

namespace Instagram.ViewModels;

public class RegisterViewModel
{
    [DataType(DataType.Upload, ErrorMessage = "загрузите фотку")]
    [Required(ErrorMessage = "Загрузите Обложку")]
    [Display(Name = "Загрутите Обложку")]
    [AllowedExtensions(new []{".png" , ".jpeg" , ".jpg" , ".webp" , ".ico" , ".svg"})]
    public IFormFile Avatar { get; set; }
    
    public string? PathFile { get; set; }

    public string? Name { get; set; }

    public string? About { get; set; }

    public string? Male { get; set; }
    [RegularExpression(@"^\+7\s?\(?\d{3}\)?\s?\d{3}\s?\-?\d{2}\-?\d{2}$", ErrorMessage = "Пожалуйста, введите номер телефона в формате +7 (XXX) XXX XX XX")]
    [DataType(DataType.PhoneNumber , ErrorMessage = "тут должен быть только номер телефона")]
    public string? Phone { get; set; }

    [DataType(DataType.Password,ErrorMessage = "Тут должен быть только пароль")]
    [Required(ErrorMessage = "Укажите пароль")]
    [Display(Name = "Введите пароль")]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[a-zA-Z0-9]{8,}$", ErrorMessage = "Пароль должен содержать в себе 1 букву верхнего регистра, 1 букву нижнего регистра, а также минимум 1 цифру, длина пароля не менее 8 символов.")]

    public string Password { get; set; }
    
    [RegularExpression (@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
    [DataType(DataType.EmailAddress , ErrorMessage = "тут должен быть только email")]
    [Required(ErrorMessage = "Заполните Email")]
    [Display(Name = "Укажите Email адрес")]

    public string Email { get; set; }
    [DataType(DataType.Text , ErrorMessage = "тут должен быть только текст")]
    [Required(ErrorMessage = "Заполните логин")]
    [Display(Name = "Укажите ваш логин")]

    public string Login { get; set; }
    
    [DataType(DataType.Password,ErrorMessage = "Тут должен быть только пароль")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [Required(ErrorMessage = "Укажите пароль")]
    [Display(Name = "Введите пароль")]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[a-zA-Z0-9]{8,}$", ErrorMessage = "Пароль должен содержать в себе 1 букву верхнего регистра, 1 букву нижнего регистра, а также минимум 1 цифру, длина пароля не менее 8 символов.")]
    public string ConfirmPassword { get; set; }

}