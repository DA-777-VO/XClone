namespace XClone.Api.Entities;

public class Like
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid TweetId { get; set; }
    public Tweet Tweet { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}