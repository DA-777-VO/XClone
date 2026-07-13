namespace XClone.Api.Entities;

public class Subscription
{
    public Guid FollowerId { get; set; }
    public User Follower { get; set; }

    public Guid FolloweeId { get; set; }
    public User Followee { get; set; }

    public DateTime SubscribedAt { get; set; }
}
