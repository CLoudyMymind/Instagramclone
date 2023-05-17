using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers;

[Authorize]
public class PostsController : Controller
{
    private readonly IAccountService _accountService;
    private readonly UserManager<User> _userManager;
    private readonly IPostService _postService;
    private readonly IFileService _fileService;
    private readonly ICommentService _commentService;
    private readonly ILikeService _likeService;
    private readonly IFollowingService _followingService;

    public PostsController(IAccountService accountService, UserManager<User> userManager, IPostService postService,
        IFileService fileService, ICommentService commentService, ILikeService likeService,
        IFollowingService followingService)
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
    public IActionResult PostsCreate()
    {
        return View(new CreatePostViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostsCreate(CreatePostViewModel model)
    {
        if (ModelState.IsValid)
        {
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            var mimeType = MimeTypes.GetMimeType(Path.GetExtension(model.Avatar.FileName));
            if (!allowedMimeTypes.Contains(mimeType))
            {
                TempData["Error"] = "Вы пытаетесь загрузить файл у которого разрешение которое не допустипо\n" +
                                    "Допустимые форматы png jpeg jpg";
                return View(model);
            }

            var imageUrl = Url.Content($"/postImages/{await _fileService.FilePostCheck(model)}");
            var userData = HttpContext.User;
            await _postService.CreatePost(userData, model, imageUrl);
        }

        return RedirectToAction("AboutProfile", "Profiles");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddСomment(InfoByPostViewModel model, string Url)
    {
        try
        {
            await _commentService.Create(model, HttpContext.User);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Произошла ошибка попробуйте в другой раз");
            throw;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Like(InfoByPostViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "Такого пользователя нету";
            return RedirectToAction("register", "Account");
        }

        var result = await _likeService.LikeAsync(model.Post.Id, user.Id);
        if (result == false) return NotFound();
        try
        {
            return Redirect(Request.Headers["Referer"].ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine("Такого айди нету в базе");
            throw;
        }
    }

    [HttpGet]
    public IActionResult DeletePost(string id)
    {
        var data = _postService.Delete(HttpContext.User, id);
        if (data == false)
        {
            TempData["Error"] = "Вы пытаетесь удалить чужой пост или которого нету";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        return RedirectToAction("AboutProfile", "Profiles");
    }

    [HttpGet]
    public IActionResult EditPost(string id)
    {
        var data = _postService.GetAll().FirstOrDefault(p => p.Id == id);
        var dataPost = new CreatePostViewModel
        {
            Decription = data.Decription,
            Post = data
        };
        return View(dataPost);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditPost(CreatePostViewModel data)
    {
        _postService.Edit(data, data.Post.Id);
        return RedirectToAction("AboutProfile" , "Profiles");
    }
}