using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<User> _userManager;
    private readonly IPostService _postService;
    private readonly IFileService _fileService;


    public AccountController(
        IAccountService accountService,
        UserManager<User> userManager,
        IPostService postService,
        IFileService fileService
    )
    {
        _accountService = accountService;
        _userManager = userManager;
        _postService = postService;
        _fileService = fileService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View(await _accountService.IndexPage(HttpContext.User));
    }

    [Authorize]
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
        var isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated)
            return RedirectToAction("Login");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated) return RedirectToAction("Login");

        if (ModelState.IsValid)
        {
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            var mimeType = MimeTypes.GetMimeType(Path.GetExtension(model.Avatar.FileName));
            if (!allowedMimeTypes.Contains(mimeType))
            {
                TempData["Error"] = "Вы загружаете формат который не позволен допустимые форматы jpg jpeg png";
                return RedirectToAction("Register");
            }

            var imageUrl = Url.Content($"/images/{await _fileService.FileRegisterCheck(model)}");
            model.PathFile = imageUrl;
            var result = await _accountService.RegisterUserAsync(model);
            if (result.Succeeded) return RedirectToAction("Index");
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
        bool isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            return RedirectToAction("Login");
        }
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
        bool isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            return RedirectToAction("Login");
        }
        else
        {
            if (ModelState.IsValid)
            {
                var isUserExists = await _accountService.IsUserExistsAsync(model);
                if (isUserExists)
                {
                    var passwordValid = await _accountService.CheckUserPasswordAsync(model, model.Password);
                    if (passwordValid)
                        try
                        {
                            var signInResult = await _accountService.LoginUserAsync(model);
                            if (signInResult.Succeeded)
                            {
                                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                                    return Redirect(model.ReturnUrl);

                                return RedirectToAction("Index");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                }

                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        bool isAuthenticated = User.Identity.IsAuthenticated;
        if (isAuthenticated)
        {
            await _accountService.SignOutUserAsync();
            return RedirectToAction("Index");
        }
        else
        {
            return RedirectToAction("Login");
        }
    }
}