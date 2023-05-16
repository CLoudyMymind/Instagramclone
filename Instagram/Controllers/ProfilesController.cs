using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers;

[Authorize]
public class ProfilesController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IProfileService _profileService;
    private readonly UserManager<User> _userManager;
    private readonly IPostService _postService;
    private readonly IFileService _fileService;
    private readonly ICommentService _commentService;
    private readonly ILikeService _likeService;
    private readonly IFollowingService _followingService;

    public ProfilesController(IAccountService accountService, UserManager<User> userManager, IFileService fileService,
        IPostService postService, ICommentService commentService, ILikeService likeService,
        IFollowingService followingService, IProfileService profileService)
    {
        _accountService = accountService;
        _userManager = userManager;
        _fileService = fileService;
        _postService = postService;
        _commentService = commentService;
        _likeService = likeService;
        _followingService = followingService;
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> About(string Id)
    {
        var viewModel = await _profileService.GetAboutViewModel(Id, User);
        return View(viewModel);
    }

    [Route("Abouts-Profile")]
    [HttpGet]
    public async Task<IActionResult> AboutProfile()
    {
        var viewModel = await _profileService.GetAboutViewModel(User);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> AboutsPostData(string id)
    {
        if (_postService.GetAll().Any(p => p!.Id == id))
        {
            var appUser = await _userManager.GetUserAsync(User);
            InfoByPostViewModel postComment = new()
            {
                Post = _postService.GetByIdPostAll(id),
                User = appUser
            };
            return View(postComment);
        }

        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> Follow(string id)
    {
        var isFollowed = await _followingService.FollowOrUnfollowUserAsync(id, User);
        if (isFollowed == false)
        {
            TempData["Error"] = "Вы пытаетесь подписаться на себя";
            return RedirectToAction("About", new { Id = id });
        }

        return RedirectToAction("About", new { id });
    }

    [HttpGet]
    public IActionResult AllFollowersUser(string id)
    {
        return View(_profileService.FollowUsers(id));
    }

    [HttpGet]
    public IActionResult AllFlowingsUsers(string id)
    {
        return View(_profileService.FlowingUsers(id));
    }
}