using System.Security.Claims;
using Instagram.Models;
using Instagram.ViewModels;

namespace Instagram.services.Abstractions;

public interface IProfileService
{
    Task<AboutsViewModel> GetAboutViewModel( ClaimsPrincipal user);
    Task<AboutsViewModel> GetAboutViewModel(string id, ClaimsPrincipal user);
    List<FollowersAndSubscribers> FollowUsers(string id);
    List<FollowersAndSubscribers> FlowingUsers(string id);
}