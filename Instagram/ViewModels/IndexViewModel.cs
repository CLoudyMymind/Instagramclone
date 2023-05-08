using Instagram.Models;

namespace Instagram.ViewModels;

public class IndexViewModel
{
    public List<User> Users { get; set; }

    public SearchUserViewModel SearchUserViewModel { get; set; }
    
    public string Comment { get; set; }

    public List<FollowersAndSubscribers> Follows { get; set; }
    public Post Post { get; set; }
}