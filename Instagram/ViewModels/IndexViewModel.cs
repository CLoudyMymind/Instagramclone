using System.ComponentModel.DataAnnotations;
using Instagram.Models;

namespace Instagram.ViewModels;

public class IndexViewModel
{
    public List<User>? Users { get; set; }

    public SearchUserViewModel? SearchUserViewModel { get; set; }
    [DataType(DataType.Text , ErrorMessage = "тут должен быть только текст")]
    [Required(ErrorMessage = "Заполните Коментарий")]
    public string Comment { get; set; }

    public List<FollowersAndSubscribers>? Follows { get; set; }
    public Post? Post { get; set; }

}