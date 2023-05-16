using System.Security.Claims;
using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class PostService : IPostService
{
    private readonly UserManager<User> _userManager;
    private readonly InstagramContext _instagramContext;
    private readonly IHostEnvironment _appEnvironment;


    public PostService(UserManager<User> userManager, InstagramContext instagramContext, IHostEnvironment appEnvironment)
    {
        _userManager = userManager;
        _instagramContext = instagramContext;
        _appEnvironment = appEnvironment;
    }


    public async Task CreatePost(ClaimsPrincipal user, CreatePostViewModel model, string img)
    {


        var searchUserData = await _userManager.GetUserAsync(user);
        Post post = new Post()
        {
            Id = Guid.NewGuid().ToString(),
            Creater =  searchUserData,
            Decription = model.Decription,
            CreateTime = DateTime.Now.ToUniversalTime(),
            PathFile = img,
            CreaterId =  searchUserData!.Id
        };
          _instagramContext.Posts.Add(post);
        await _instagramContext.SaveChangesAsync();
            
        searchUserData.Posts.Add(post);
        await _userManager.UpdateAsync(searchUserData);
         
    }

    public List<Post> About(ClaimsPrincipal user)
    {
        var users = _userManager.GetUserId(user);
        var post = _instagramContext.Posts
            .Include(post => post.Likes)
            .Include(post => post.Comments)
            .Where(post => post.Creater!.Id == users)
            .ToList();
        return post;
    }
    
    public Post? GetByIdPostAll(string id) 
    {
          return _instagramContext.Posts
                        .Include(post => post.Creater)
                        .Include(post => post.Likes)
                        .Include(post => post.Comments)
                        .FirstOrDefault(post => post.Id == id);
    }
    public string GetByIdLikesByPost(InfoByPostViewModel model)
    {

        try
        {
            var str= _instagramContext.Posts.Include(p => p.Creater).FirstOrDefault(p => p.Id ==  model.Post.Id)!;
            return str.Creater.Id;

        }
        catch (Exception e)
        {
            Console.WriteLine("Такого айди нету в базе");
            throw;
        }


    }

    public IQueryable<Post> GetById(string id)
    {
        
        return _instagramContext.Posts.Include(p => p.Creater).Where(u => u.Id == id);
    }

    public List<Post> GetAll()
    {
       return _instagramContext.Posts.Include(p => p.Creater).ToList();
    }
    public bool Delete(ClaimsPrincipal user, string id)
    {
        var userId = _userManager.GetUserId(user);
        var post = _instagramContext.Posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return false;
        }
        if (post.CreaterId != userId)
        {
            return false;
        }
        var likes = _instagramContext.Likes.Where(l => l.Post.Id == id);
        var comments = _instagramContext.Comments.Where(c => c.PostId == id);
        _instagramContext.Likes.RemoveRange(likes);
        _instagramContext.Comments.RemoveRange(comments);
        _instagramContext.Posts.Remove(post);
        _instagramContext.SaveChanges();
        return true;
    }

    public void Edit(CreatePostViewModel model, string postId)
    {
        var post = _instagramContext.Posts.FirstOrDefault(p => p.Id == postId);
        if (post != null)
        { 
            _instagramContext.Update(post.Decription = model.Decription);
            _instagramContext.SaveChanges();
        }
    }
}