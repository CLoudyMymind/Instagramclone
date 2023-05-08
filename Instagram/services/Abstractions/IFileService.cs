using System.Security.Claims;
using Instagram.ViewModels;

namespace Instagram.services.Abstractions;

public interface IFileService
{
    Task<string> FilePostCheck(CreatePostViewModel model);

    Task<string> FileRegisterCheck(RegisterViewModel model);
    Task<string> FileEditCheck(EditProfileViewModel model, ClaimsPrincipal user);
}