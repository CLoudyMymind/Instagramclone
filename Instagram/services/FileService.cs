using System.Security.Claims;
using Instagram.Models;
using Instagram.services.Abstractions;
using Instagram.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Instagram.services;

public class FileService : IFileService
{
    private readonly IHostEnvironment _appEnvironment;
    private readonly InstagramContext _instagramContext;
    private readonly UserManager<User> _userManager;
    public FileService(IHostEnvironment appEnvironment, InstagramContext instagramContext, UserManager<User> userManager)
    {
        _appEnvironment = appEnvironment;
        _instagramContext = instagramContext;
        _userManager = userManager;
    }

    public  async Task<string> FilePostCheck(CreatePostViewModel model)
    {
        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(wwwrootPath, "postImages");
        if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
        var fileName =  Guid.NewGuid() + Path.GetExtension(model.Avatar.FileName);
        var filePath = Path.Combine(_appEnvironment.ContentRootPath, "wwwroot/postImages", fileName);
       await  using var stream = new FileStream(filePath, FileMode.Create);
       await  model.Avatar.CopyToAsync(stream);
       return fileName;
    }

    public async Task<string> FileRegisterCheck(RegisterViewModel model)
    {
        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesPath = Path.Combine(wwwrootPath, "images");
        if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
        var fileName =  Guid.NewGuid() + Path.GetExtension(model.Avatar.FileName);
        var filePath = Path.Combine(_appEnvironment.ContentRootPath, "wwwroot/images", fileName);
        await  using var stream = new FileStream(filePath, FileMode.Create);
        await  model.Avatar.CopyToAsync(stream);
        return fileName;
    }

    public async Task<string> FileEditCheck(EditProfileViewModel model, ClaimsPrincipal user)
    {
        
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesPath = Path.Combine(wwwrootPath, "images");
            if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
            var fileName =  Guid.NewGuid() + Path.GetExtension(model.Avatar.FileName);
            var filePath = Path.Combine(_appEnvironment.ContentRootPath, "wwwroot/images", fileName);
            await  using var stream = new FileStream(filePath, FileMode.Create);
            await  model.Avatar.CopyToAsync(stream);
            return fileName;
    }
}