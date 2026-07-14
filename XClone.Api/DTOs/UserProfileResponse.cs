namespace XClone.Api.DTOs;

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Bio { get; set; }
    public int TweetsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}