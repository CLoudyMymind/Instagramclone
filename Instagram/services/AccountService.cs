using System.Security.Claims;
using Instagram.Extensions;
using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly InstagramContext _instagramContext;

    public AccountService(
        UserManager<User> userManager,
        SignInManager<User> signInManager, InstagramContext instagramContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _instagramContext = instagramContext;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
    {
        
        User user = new()
        {
            Email = model.Email,
            UserName = model.Login,
            Male = model.Male,
            Name = model.Name,
            PhoneNumber = model.Phone,
            About = model.About,
            PathFile = model.PathFile!
        };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
        }
        return result;
    }

    public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
    {
        if (model.Name != null)
        {
            User? user = await _userManager.FindByNameAsync(model.Name) ?? await _userManager.FindByEmailAsync(model.Name);
            if (user != null)
            {
                SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false
                );
                return signInResult;
            }
        }
        throw new Exception("Такого пользователя нету");
    }


    public async Task SignOutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> IsUserExistsAsync(LoginViewModel email)
    {
        if (email.Name is not null)
        {
            User? user = await _userManager.FindByNameAsync(email.Name) ?? await _userManager.FindByEmailAsync(email.Name);
            return user != null;
        }
        return false;
    }

   public async Task<bool> CheckUserPasswordAsync(LoginViewModel model, string password)
   {
       if (model.Name != null)
       {
           User? user = await _userManager.FindByNameAsync(model.Name) ?? await _userManager.FindByEmailAsync(model.Name);
           if (user != null)
           {
               bool passwordValid = await _userManager.CheckPasswordAsync(user, password);
               return passwordValid;
           }
       }
       return false;
   }
   public List<User> GetAll() => _instagramContext.Users.Include(p => p.Posts).ToList();
   public List<User> Search(SearchUserViewModel filter)
    {

        var str = _instagramContext.Users
            .WhereIf(!string.IsNullOrEmpty(filter.UserName), u =>
                u.UserName.Contains(filter.UserName)
                || u.Email.Contains(filter.UserName)
                || (u.Name != null && u.Name.Contains(filter.UserName))).ToList();
        return str.ToList();
    }
   public async Task<IndexViewModel> IndexPage(ClaimsPrincipal user)
    {
        var userId = (await _userManager.GetUserAsync(user))?.Id;
        if (userId == null) return new IndexViewModel();

        var followedUsers = await _instagramContext.FollowersAndSubscribers
            .Include(follow => follow.FollowingUser)
            .Include(follow => follow.FollowerUser.Posts)
            .ThenInclude(post => post.Likes)
            .Include(follow => follow.FollowerUser.Posts)
            .ThenInclude(post => post.Comments)
            .Where(follow => follow.FollowingUser.Id == userId)
            .ToListAsync();

        var allUserIds = await _instagramContext.Users.Select(u => u.Id).ToListAsync();
        var followingUserIds = await _instagramContext.FollowersAndSubscribers
            .Where(follow => follow.FolowingUserId == userId)
            .Select(follow => follow.FollowerUserId)
            .ToListAsync();
        var usersToExclude = followingUserIds.Concat(new[] { userId });
        var users = await _instagramContext.Users
            .Where(u => !usersToExclude.Contains(u.Id))
            .ToListAsync();

        var model = new IndexViewModel
        {
            Follows = followedUsers,
            Users = users
        };
        return model;
    }

}
