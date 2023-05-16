using System.Security.Claims;
using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class ProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly InstagramContext _instagramContext;

    public ProfileService(UserManager<User> userManager, InstagramContext instagramContext)
    {
        _userManager = userManager;
        _instagramContext = instagramContext;
    }

    public async Task<AboutsViewModel> GetAboutViewModel(ClaimsPrincipal user)
    {
        User appUser = await _userManager.GetUserAsync(user);
        appUser.Posts = await _instagramContext.Posts
            .Include(post => post.Likes)
            .Include(post => post.Comments)
            .Where(post => post.Creater!.Id == appUser.Id).OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        var follow = await GetFollowersAndSubscribersForUserQuery(appUser.Id)
            .Where(follow => follow.FollowingUser.Id == appUser.Id).OrderByDescending(c => c.CreateDate)
            .ToListAsync();

        var following = await GetFollowersAndSubscribersForUserQuery(appUser.Id)
            .Where(follow => follow.FollowerUser.Id == appUser.Id).OrderByDescending(c => c.CreateDate)
            .ToListAsync();
        return new AboutsViewModel
        {
            User = appUser,
            Followers = follow,
            Followings = following
        };
    }
    private IQueryable<FollowersAndSubscribers> GetFollowersAndSubscribersForUserQuery(string userId)
    {
        return _instagramContext.FollowersAndSubscribers
            .Include(follow => follow.FollowerUser)
            .Include(follow => follow.FollowingUser)
            .Where(follow => follow.FollowingUser.Id == userId || follow.FollowerUser.Id == userId);
    }
    public async Task<AboutsViewModel> GetAboutViewModel(string? id, ClaimsPrincipal user)
    {
        User appUser = id is null ? await _userManager.GetUserAsync(user) : await _userManager.FindByIdAsync(id);
        appUser.Posts = await _instagramContext.Posts
            .Include(post => post.Likes)
            .Include(post => post.Comments)
            .Where(post => post.Creater!.Id == appUser.Id)
            .ToListAsync();

        var follow = await GetFollowersAndSubscribersForUserQuery(appUser.Id)
            .Where(follow => follow.FollowingUser.Id == appUser.Id)
            .ToListAsync();

        var following = await GetFollowersAndSubscribersForUserQuery(appUser.Id)
            .Where(follow => follow.FollowerUser.Id == appUser.Id)
            .ToListAsync();
        return new AboutsViewModel
        {
            User = appUser,
            Followers = follow,
            Followings = following
        };
    }

    public List<FollowersAndSubscribers> FollowUsers(string id)
    {
        return GetFollowersAndSubscribersQuery(id, true)
            .Include(follow => follow.FollowingUser)
            .OrderByDescending(c => c.CreateDate)
            .ToList();
    }

    public List<FollowersAndSubscribers> FlowingUsers(string id)
    {
        return GetFollowersAndSubscribersQuery(id, false)
            .Include(follow => follow.FollowerUser)
            .OrderByDescending(c => c.CreateDate)
            .ToList();
    }
    private IQueryable<FollowersAndSubscribers> GetFollowersAndSubscribersQuery(string id, bool searchFollowers)
    {
        var query = _instagramContext.FollowersAndSubscribers
            .Where(follow => searchFollowers ? follow.FollowerUser.Id == id : follow.FollowingUser.Id == id);

        return query;
    }
}