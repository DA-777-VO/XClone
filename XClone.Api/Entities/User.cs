using System.Text.Json.Serialization;

namespace XClone.Api.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Bio { get; set; }

    [JsonIgnore]
    public List<Tweet> Tweets { get; set; } = new List<Tweet>();

    [JsonIgnore]
    public List<Like> Likes { get; set; } = new List<Like>();

    [JsonIgnore]
    public List<Subscription> Followers { get; set; } = new List<Subscription>();
    [JsonIgnore]
    public List<Subscription> Following { get; set; } = new List<Subscription>();
}
