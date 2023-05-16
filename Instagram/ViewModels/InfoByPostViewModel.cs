using System.ComponentModel.DataAnnotations;
using Instagram.Models;

namespace Instagram.ViewModels;

public class InfoByPostViewModel
{
    
        [DataType(DataType.Text , ErrorMessage = "тут должен быть только текст")]
        [Required(ErrorMessage = "Заполните коментарий")]
        public string Comment { get; set; }
        public Post? Post { get; set; }
        public User? User { get; set; }

    
}