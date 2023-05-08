using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class LikeService : ILikeService
{
    private readonly InstagramContext _instagramContext;

    public LikeService(InstagramContext instagramContext)
    {
        _instagramContext = instagramContext;
    }

    public async Task<bool> LikeAsync(string postId, string userId)
    {
        var post = await _instagramContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        if (post is null) return false;

        var isLikeChecked = _instagramContext.Likes.Any(l => l.LikeUserId == userId && l.LikedPostId == postId);
        if (isLikeChecked)
        {
            var likeRemove = _instagramContext.Likes.FirstOrDefault(like1 => like1.LikeUserId == userId && like1.LikedPostId == postId);
            _instagramContext.Likes.Remove(likeRemove);
            await _instagramContext.SaveChangesAsync();
            return true;
        }

        Like like = new()
        {
            Id = Guid.NewGuid().ToString(),
            LikeUserId = userId,
            LikeUser = await _instagramContext.Users.FindAsync(userId),
            Post = post,
            LikedPostId = postId
        };
        _instagramContext.Likes.Add(like);
        await _instagramContext.SaveChangesAsync();
        return true;
    }

}