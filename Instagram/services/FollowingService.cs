using System.Security.Claims;
using Instagram.Models;
using Instagram.services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class FollowingService : IFollowingService
{
    private readonly InstagramContext _instagramContext;
    private readonly UserManager<User> _userManager;

    public FollowingService(InstagramContext instagramContext, UserManager<User> userManager)
    {
        _instagramContext = instagramContext;
        _userManager = userManager;
    }

    public async Task<FollowersAndSubscribers?> SearchUser(string Id)
    {
       
        return await _instagramContext.FollowersAndSubscribers.FindAsync(Id);
    }
    public async Task<bool> FollowOrUnfollowUserAsync(string userId, ClaimsPrincipal user)
    {
        var followerUser = await _instagramContext.Users.FindAsync(userId);
        var following = await _userManager.GetUserAsync(user);

        var isFollowing = _instagramContext.FollowersAndSubscribers
            .Any(f => f.FollowerUser.Id == userId && f.FollowingUser.Id == following.Id);
        if (userId == following.Id)
        {
            return false;
        }
        if (isFollowing)
        {
            var followToRemove = await _instagramContext.FollowersAndSubscribers
                .FirstOrDefaultAsync(f => f.FollowerUser.Id == userId && f.FollowingUser.Id == following.Id);
        
            _instagramContext.FollowersAndSubscribers.Remove(followToRemove);
            await _instagramContext.SaveChangesAsync();
            return true;
        }
        else
        {
            var follow = new FollowersAndSubscribers
            {
                Id = Guid.NewGuid().ToString(),
                FollowerUser = followerUser,
                FollowerUserId = followerUser.Id,
                FollowingUser = following,
                FolowingUserId = following.Id
            };
            _instagramContext.FollowersAndSubscribers.Add(follow);
            await _instagramContext.SaveChangesAsync();
            return true;
        }

    }
    public async Task<string> GetById(string id)
    {
        
        var subscriber = (await _instagramContext.Users.FindAsync(id))!;
        return subscriber.Id;
    }
}
  
