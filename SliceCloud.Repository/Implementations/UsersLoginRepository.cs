using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SliceCloud.Repository.Interfaces;
using SliceCloud.Repository.Models;

namespace SliceCloud.Repository.Implementations;

public class UsersLoginRepository(SliceCloudContext context) : IUsersLoginRepository
{
    private readonly SliceCloudContext _context = context;

    public async Task<UsersLogin?> GetUserLoginByEmailAsync(string userEmail)
    {
        return await _context.UsersLogins.FirstOrDefaultAsync(u => u.Email!.ToLower() == userEmail.ToLower());
    }

    public async Task<UsersLogin?> GetUserLoginAsync(string userEmail, string userHashedPassword)
    {
        UsersLogin? usersLogin = await _context.UsersLogins.Include(u => u.User).FirstOrDefaultAsync(
            u => u.Email == userEmail
            && u.PasswordHash == userHashedPassword
            && u.IsFirstLogin == false
            && u.User!.IsDeleted == false
            && u.User.Status == 1
            );
        return usersLogin;
    }

    public async Task SavePasswordResetTokenAsync(int userId, string passwordResetToken, DateTime expiration, bool isUsed)
    {
        UsersLogin? user = await _context.UsersLogins.FindAsync(userId);
        if (user != null)
        {
            user.ResetToken = passwordResetToken;
            user.ResetTokenExpiration = expiration;
            user.IsResetTokenUsed = isUsed;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<UsersLogin?> GetUserByResetTokenAsync(string resetToken)
    {
        return await _context.UsersLogins.FirstOrDefaultAsync(u => u.ResetToken == resetToken);
    }

    public async Task<bool> SetUserPasswordAsync(int userLoginId, string newPassword)
    {
        UsersLogin? user = await _context.UsersLogins.FindAsync(userLoginId);
        if (user == null)
        {
            return false;
        }
        User? userTable = await _context.Users.FindAsync(user.UserId);
        if (userTable == null)
        {
            return false;
        }
        user.PasswordHash = newPassword;
        userTable.PasswordHash = newPassword;

        _context.UsersLogins.Update(user);
        _context.Users.Update(userTable);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> InvalidateResetTokenAsync(int userLoginId)
    {
        UsersLogin? usersLogin = await _context.UsersLogins
       .FirstOrDefaultAsync(u => u.UserLoginId == userLoginId);

        if (usersLogin == null)
            return false;

        usersLogin.IsResetTokenUsed = true;
        await _context.SaveChangesAsync();

        return true;
    }

}
