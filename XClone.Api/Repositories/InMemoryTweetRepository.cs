// using XClone.Api.Entities;
//
// namespace XClone.Api.Repositories;
//
// public class InMemoryTweetRepository : ITweetRepository
// {
//     
//     private readonly List<Tweet> _tweets = new List<Tweet>();
//     
//     
//     public async Task Add(Tweet tweet)
//     {
//         _tweets.Add(tweet);
//     }
//
//     public async Task<List<Tweet>> GetAll()
//     {
//         return _tweets;
//     }
// }