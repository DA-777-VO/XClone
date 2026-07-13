using XClone.Api.Entities;
using XClone.Api.Repositories;

namespace XClone.Api.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ToggleFollowAsync(Guid followerId, Guid followeeId)
    {
        if (followerId == followeeId) throw new ArgumentException("Нельзя подписаться на самого себя");
        
        var followee = await _userRepository.GetByIdAsync(followeeId);
        if (followee == null) throw new ArgumentException("Пользователь не найден");
        
        Subscription sub = await _userRepository.GetSubscriptionAsync(followerId, followeeId);
        if (sub != null)
        {
            await _userRepository.RemoveSubscriptionAsync(sub);
        }
        else
        {
            Subscription newSub = new Subscription
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                SubscribedAt = DateTime.Now
            };
            await _userRepository.AddSubscriptionAsync(newSub);
        }
    }
}