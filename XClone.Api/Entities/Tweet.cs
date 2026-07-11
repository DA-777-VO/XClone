using System.Text.Json.Serialization;

namespace XClone.Api.Entities;

public class Tweet
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; }
    
    [JsonIgnore]
    public List<Like> Likes { get; set; } = new List<Like>();
}