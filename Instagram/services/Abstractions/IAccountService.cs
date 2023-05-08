using System.Security.Claims;
using Instagram.Models;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Instagram.services.Abstractions;

public interface IAccountService
{
    public Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);

    public  Task<SignInResult> LoginUserAsync(LoginViewModel model);
    public Task SignOutUserAsync();

    public Task<bool> IsUserExistsAsync(LoginViewModel email);

    public Task<bool> CheckUserPasswordAsync(LoginViewModel model, string password);

    Task<List<string?>> CheсkEmail(string email);
    
    List<User> GetAll();

   User? GetById(string id);
   public Task<IdentityResult> Edit(EditProfileViewModel model);
   IQueryable<User> GetQueryableUsers();

   Task<AboutsViewModel> GetAboutViewModel(string? id, ClaimsPrincipal user);
   public List<User> AllUsersAbout(ClaimsPrincipal? user);
   List<User> Search(SearchUserViewModel filter);

   List<FollowersAndSubscribers> FollowUsers(string id);
   List<FollowersAndSubscribers> FlowingUsers(string id);
   Task<IndexViewModel> IndexPage(ClaimsPrincipal user);
}