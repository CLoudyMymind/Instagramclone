using System.Security.Claims;
using Instagram.Models;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Instagram.services.Abstractions;

public interface ICommentService
{
    public List<Comment> GetAll();

    public Task  Create(InfoByPostViewModel model , ClaimsPrincipal user);
    string GetByIdComment(InfoByPostViewModel model);
}