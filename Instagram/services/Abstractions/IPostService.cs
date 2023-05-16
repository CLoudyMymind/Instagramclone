using System.Security.Claims;
using Instagram.Models;
using Instagram.ViewModels;

namespace Instagram.services.Abstractions;

public interface IPostService
{
    public Task CreatePost(ClaimsPrincipal user, CreatePostViewModel model, string urlPath);
    public  List<Post> About(ClaimsPrincipal user);
    public Post GetByIdPostAll(string id);
    public string GetByIdLikesByPost(InfoByPostViewModel model);
    public IQueryable<Post> GetById(string id);
    public List<Post> GetAll();
    public bool Delete(ClaimsPrincipal user, string id);

    public void Edit(CreatePostViewModel model, string postId);
}