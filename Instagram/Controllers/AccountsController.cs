using Instagram.Extensions;
using Instagram.Models;
using Instagram.services;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Instagram.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<User> _userManager;
    private readonly IPostService _postService;
    private readonly IFileService _fileService;
    private readonly ICommentService _commentService;
    private readonly ILikeService _likeService;
    private readonly IFollowingService _followingService;
    

    public AccountController(
        IAccountService accountService, 
        UserManager<User> userManager,
        IPostService postService,  
        IFileService fileService,
        ICommentService commentService, 
        ILikeService likeService, IFollowingService followingService)
    {
        _accountService = accountService;
        _userManager = userManager;
        _postService = postService;
        _fileService = fileService;
        _commentService = commentService;
        _likeService = likeService;
        _followingService = followingService;
       
    }

    [HttpGet]
    [Authorize]
    public  async Task<IActionResult> Index()
    {
       
        return View(await _accountService.IndexPage(HttpContext.User));
    }

    public IActionResult SearchUser(SearchUserViewModel filter)
    {
        filter.UserName.ToUpper();
        var str = _accountService.Search(filter);
        var data = new IndexViewModel
        {
            Users = str.ToList()
        };
        return !str.Any() ? View(data) : View(data);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" ,"image/webp" };
            var mimeType = MimeTypes.GetMimeType(Path.GetExtension(model.Avatar.FileName));
            if (!allowedMimeTypes.Contains(mimeType))
            {
                TempData["Error"] = "Вы загружаете формат который не позволен допустимые форматы jpg jpeg png";
                return RedirectToAction("Register");
            }
            var imageUrl = Url.Content($"/images/{ await _fileService.FileRegisterCheck(model)}");
            model.PathFile = imageUrl;
            var result = await _accountService.RegisterUserAsync(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    TempData["Error"] = "Этот никнейм уже занят используйте другой";
                    return RedirectToAction("Register", "Account");
                }
                if (error.Code == "DuplicateEmail")
                {
                    TempData["Error"] = "Этот email уже занят используйте другой";
                    return RedirectToAction("Register", "Account");
                }
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var isUserExists = await _accountService.IsUserExistsAsync(model);
            if (isUserExists)
            {
                var passwordValid = await _accountService.CheckUserPasswordAsync(model, model.Password);
                if (passwordValid)
                {
                    try
                    {
                        var signInResult = await _accountService.LoginUserAsync(model);
                        if (signInResult.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            {
                                return Redirect(model.ReturnUrl);
                            }

                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
        }
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        await _accountService.SignOutUserAsync();
        return RedirectToAction("Index");
    }
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Edit(string id)
    {
        if (_accountService.GetAll().Any(s => s.Id == id))
            try
            {
                var user = _accountService.GetById(id);
                return View(new EditProfileViewModel()
                {
                    About = user.About,
                    PathFile = user.PathFile,
                    Name = user.Name,
                    Avatar = user.Avatar,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    Phone = user.PhoneNumber,
                    Login = user.UserName,
                    Male = user.Male
                });

            }
            catch (Exception e)
            {
                return RedirectToAction("About");
            }

        return NotFound();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Avatar == null)
            {
                var str = _accountService.GetAll().FirstOrDefault(p => p.Id == model.Id);
                model.PathFile = str.PathFile;
            }
            else
            {
                var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                var mimeType = MimeTypes.GetMimeType(Path.GetExtension(model.Avatar.FileName));
                if (!allowedMimeTypes.Contains(mimeType))
                {
                    TempData["Error"] = "Вы загружаете формат который не позволен допустимые форматы jpg jpeg png";
                    return RedirectToAction("Edit" , new {Id = model.Id});
                }
                var imageUrl = Url.Content($"/images/{ await _fileService.FileEditCheck(model, HttpContext.User)}");
                model.PathFile = imageUrl;
            }
            var result = await _accountService.Edit(model);
            if (result.Succeeded)
            {
                return RedirectToAction("About");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateUserName")
                {
                    TempData["Error"] = "Этот никнейм уже занят используйте другой";
                    return RedirectToAction("Register", "Account");
                }
                if (error.Code == "DuplicateEmail")
                {
                    TempData["Error"] = "Этот email уже занят используйте другой";
                    return RedirectToAction("Register", "Account");
                }
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult PostsCreate()
    {
        return View(new CreatePostViewModel());
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> PostsCreate(CreatePostViewModel model)
    {
        if (ModelState.IsValid)
        {
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png" , "image/webp"};
            var mimeType = MimeTypes.GetMimeType(Path.GetExtension(model.Avatar.FileName));
            if (!allowedMimeTypes.Contains(mimeType))
            {
                
                TempData["Error"] = "Вы пытаетесь загрузить файл у которого разрешение которое не допустипо\n" +
                                    "Допустимые форматы png jpeg jpg";
                return View(model);
            }
            var imageUrl =  Url.Content($"/postImages/{await _fileService.FilePostCheck(model)}");
            var userData = HttpContext.User;
          await _postService.CreatePost(userData, model, imageUrl);
        }
        return RedirectToAction("About" );
    }
[HttpGet]
public async Task<IActionResult> About(string Id)
{
    var viewModel = await _accountService.GetAboutViewModel(Id, User);
    return View(viewModel);
}


[HttpGet]
[AllowAnonymous]

public IActionResult AboutsPostData(string id)
{
    if ( _postService.GetAll().Any(p=> p.Creater!.Id == id))
    {
        InfoByPostViewModel postComment = new() { Post = _postService.GetByIdPostAll(id) };
        return View(postComment);
    }
    return NotFound();
}
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddСomment(InfoByPostViewModel model)
    {
        try
        {
            await _commentService.Create(model, HttpContext.User);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine("Произошла ошибка попробуйте в другой раз");
            throw;
        }
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Like(InfoByPostViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "Такого пользователя нету";
            return RedirectToAction("register");
        }
        var result = await _likeService.LikeAsync(model.Post.Id, user.Id);
        if (result == false) return NotFound();
        try
        {
            return RedirectToAction("index" );
        }
        catch (Exception e)
        {
            Console.WriteLine("Такого айди нету в базе");
            throw;
        }
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Follow(string id)
    {
        var isFollowed = await _followingService.FollowOrUnfollowUserAsync(id, User);
        if (isFollowed == false)
        {
            TempData["Error"] = "Вы пытаетесь подписаться на себя";
            return RedirectToAction("About", new { Id = id });
        }
        return RedirectToAction("About", new {id = id,});
    }
    
    [HttpGet]
    public IActionResult AllFollowersUser(string id)
    {
        return View(_accountService.FollowUsers(id));
    }
    [HttpGet]
    public IActionResult AllFlowingsUsers(string id)
    {
        return View(_accountService.FlowingUsers(id));
    }
    
    
}