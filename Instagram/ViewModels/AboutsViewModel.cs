using Instagram.Models;

namespace Instagram.ViewModels;

public class AboutsViewModel
{
    public User User { get; set; }
    public List<FollowersAndSubscribers> Followers { get; set; }
    public List<FollowersAndSubscribers> Followings { get; set; }
}