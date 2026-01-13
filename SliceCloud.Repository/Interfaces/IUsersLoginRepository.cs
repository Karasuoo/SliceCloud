using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Interfaces;

public interface IUsersLoginRepository
{
    Task<UsersLogin?> GetUserLoginAsync(string email, string password);
}
