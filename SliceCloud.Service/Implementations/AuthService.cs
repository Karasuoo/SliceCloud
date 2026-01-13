using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;
using SliceCloud.Service.Interfaces;

namespace SliceCloud.Service.Implementations;

public class AuthService(IUsersLoginRepository usersLoginRepository) : IAuthService
{
    private readonly IUsersLoginRepository _usersLoginRepository = usersLoginRepository;

    public async Task<UsersLogin?> AuthenticateUser(string email, string password)
    {
        UsersLogin? user = await _usersLoginRepository.GetUserLoginAsync(email, password);

        if (user == null) return null;

        return user;
    }
}
