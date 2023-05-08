namespace Instagram.Models;

public class Like
{
    public string Id { get; set; }
    
    public string LikeUserId { get; set; }
    
    public User? LikeUser { get; set; }
    public string LikedPostId { get; set; }
    
    public Post Post { get; set; }
    
    
}