using System.ComponentModel.DataAnnotations;
using Instagram.Attributes;
using Instagram.Models;

namespace Instagram.ViewModels;

public class CreatePostViewModel
{
    [DataType(DataType.Upload, ErrorMessage = "загрузите фотку")]
    [Required(ErrorMessage = "Загрузите Обложку")]
    [Display(Name = "Загрутите Обложку")]
    [AllowedExtensions(new []{".png" , ".jpeg" , ".jpg" , ".webp" , ".ico" , ".svg"})]
    public IFormFile Avatar { get; set; }
    [Required(ErrorMessage = "Заполните Инофрмацию о посте")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "Минимальное чисто символов: 1, Максимальное 150")]
    [Display(Name = "Укажите информацию о посте")]
    public string Decription { get; set; }

    public string? PathFile { get; set; }

    public Post? Post { get; set; }
}