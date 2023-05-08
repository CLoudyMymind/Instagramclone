using System.Security.Claims;
using Instagram.Models;

namespace Instagram.services.Abstractions;

public interface IFollowingService
{
    public Task<FollowersAndSubscribers?> SearchUser(string Id);

    Task<bool> FollowOrUnfollowUserAsync(string userId, ClaimsPrincipal user);
    public Task<string> GetById(string id);
}