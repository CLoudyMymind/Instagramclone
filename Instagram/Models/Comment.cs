namespace Instagram.Models;

public class Comment
{
    public Guid Id { get; set; }
    
    public string Description { get; set; }

    public DateOnly CreateDate { get; set; }
    public string ByCommentUserId { get; set; }
    
    public User User { get; set; }
    
    public string PostId { get; set; }
    
    public Post Post { get; set; }
}