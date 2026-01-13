using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersLoginRepository(SliceCloudContext context) : IUsersLoginRepository
{
    private readonly SliceCloudContext _context = context;

    public async Task<UsersLogin?> GetUserLoginAsync(string userEmail, string userPassword)
    {
        UsersLogin? user = await _context.UsersLogins.Include(u => u.User).FirstOrDefaultAsync(
            u => u.Email == userEmail
            && u.PasswordHash == userPassword
            && u.IsFirstLogin == false
            && u.User!.IsDeleted == false
            && u.User.Status == 1
            );
        return user;
    }

}
