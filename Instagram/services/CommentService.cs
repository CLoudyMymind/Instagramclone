using System.Security.Claims;
using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class CommentService : ICommentService
{
    private readonly InstagramContext _instagramContext;
    private readonly UserManager<User> _userManager;
    public CommentService(InstagramContext instagramContext, UserManager<User> userManager)
    {
        _instagramContext = instagramContext;
        _userManager = userManager;
    }

    public List<Comment> GetAll()
    {
        return _instagramContext.Comments.ToList();
    }

    public string GetByIdComment(InfoByPostViewModel model)
    {
        try
        {
            var post = _instagramContext.Posts
                .Include(post => post.Creater)
                .Include(post => post.Likes)
                .Include(post => post.Comments)
                .FirstOrDefault(post => post.Id == model.Post.Id);
            return post!.Creater!.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine("Такого айди нету в базе");
            throw;
        }
    }
    public async Task Create(InfoByPostViewModel model, ClaimsPrincipal user)
    {
        var users = await _userManager.GetUserAsync(user);
        var post = _instagramContext.Posts
            .Include(post => post.Creater)
            .Include(post => post.Likes)
            .Include(post => post.Comments)
            .FirstOrDefault(post => post.Id == model.Post.Id);

        Comment comment = new()
        {
            Description = model.Comment,
            Post = post!,
            CreateDate = DateOnly.FromDateTime(DateTime.Now),
            User = users!,
            ByCommentUserId = users!.Id,
            PostId = post!.Id
            
        };
       await _instagramContext.Comments.AddAsync(comment);
        await _instagramContext.SaveChangesAsync();
    }
}