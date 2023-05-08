namespace Instagram.Models;

public class Post
{
    public string Id { get; set; }
    
    public DateTime CreateTime { get; set; }
    
    public DateTime? Refactor { get; set; }
    public string Decription { get; set; }

    public string PathFile { get; set; }
    public User? Creater { get; set; }
    public string CreaterId { get; set; }
    
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
}