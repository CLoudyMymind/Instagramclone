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
        if (model.Email != null)
        {
            User? user = await _userManager.FindByEmailAsync(model.Email);
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                false
            );
            return signInResult;
        }

        if (model.Name != null)
        {
            User? user = await _userManager.FindByNameAsync(model.Name);
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                false
            );
            return signInResult;
        }

        throw new Exception("Такого пользователя нету");


    }

    public async Task SignOutUserAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> IsUserExistsAsync(LoginViewModel email)
    {
        if (email.Email is not null)
        {
            User? user = await _userManager.FindByEmailAsync(email.Email);
            return user != null;

        }

        if (email.Name is not null)
        {
            User? user = await _userManager.FindByNameAsync(email.Name);
            return user != null;
        }

        return false;
    }

    public async Task<bool> CheckUserPasswordAsync(LoginViewModel model, string password)
    {
        if (model.Email != null)
        {
            User? user = await _userManager.FindByEmailAsync(model.Email);
            bool passwordValid = await _userManager.CheckPasswordAsync(user, password);
            return passwordValid;

        }

        if (model.Name != null)
        {
            User? user = await _userManager.FindByNameAsync(model.Name);
            bool passwordValid = await _userManager.CheckPasswordAsync(user, password);
            return passwordValid;
        }

        return false;
    }

    public async Task<List<string?>> CheсkEmail(string email)
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var emails = allUsers.Select(u => u.Email).ToList();
        return emails;
    }



    public List<User> GetAll() => _instagramContext.Users.Include(p => p.Posts).ToList();

    public User? GetById(string id) =>  _instagramContext.Users.FirstOrDefault(u => u.Id == id);
    
    public async Task<IdentityResult> Edit(EditProfileViewModel model)
    {
        
        var fist = _instagramContext.Users.FirstOrDefault(p => p.Id == model.Id);
        if (fist is not null)
        {
            fist.Id = model.Id;
            fist.About = model.About;
            fist.Email = model.Email;
            fist.Name = model.Name;
            fist.Male = model.Male;
            fist.UserName = model.Login;
            fist.PasswordHash = model.Password;
            fist.PasswordHash = model.ConfirmPassword;

        }
        _instagramContext.Update(fist);

        IdentityResult result = await _userManager.UpdateAsync(fist);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(fist, false);
        }
        return result;
    }

    public IQueryable<User> GetQueryableUsers()
    {
        return _instagramContext.Users;
    }
    

    public List<User> AllUsersAbout(ClaimsPrincipal? user)
    {
        var userId = _userManager.GetUserId(user);
        var usersWithPosts = _instagramContext.Users.Include(u => u.Posts).FirstOrDefault(u => u.Id == userId);
        return new List<User> { usersWithPosts };
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

    public List<User> Search(SearchUserViewModel filter)
    {

        var str = _instagramContext.Users
            .WhereIf(!string.IsNullOrEmpty(filter.UserName), u =>
                u.UserName.Contains(filter.UserName)
                || u.Email.Contains(filter.UserName)
                || (u.Name != null && u.Name.Contains(filter.UserName))).ToList();
        return str.ToList();
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
