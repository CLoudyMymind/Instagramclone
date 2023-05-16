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
    List<User> Search(SearchUserViewModel filter);
    Task<IndexViewModel> IndexPage(ClaimsPrincipal user);
}