using SliceCloud.Repository.Models;

namespace SliceCloud.Service.Interfaces;

public interface IAuthService
{
    Task<UsersLogin?> AuthenticateUser(string email, string password);
}
