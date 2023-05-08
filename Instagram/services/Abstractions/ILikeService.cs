using Instagram.Models;

namespace Instagram.services.Abstractions;

public interface ILikeService
{
    Task<bool> LikeAsync(string postId, string userId);
}