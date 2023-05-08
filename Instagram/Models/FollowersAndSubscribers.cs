using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Instagram.Models;

public class FollowersAndSubscribers
{
    [Key]
    public string Id { get; set; }
    
    public string FolowingUserId { get; set; }
    public User FollowingUser { get; set; }
    
    public string FollowerUserId { get; set; }
    public User FollowerUser { get; set; }

    public DateTime CreateDate { get; set; }
}