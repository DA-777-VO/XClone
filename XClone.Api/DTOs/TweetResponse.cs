namespace XClone.Api.DTOs;

public class TweetResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
}