using System.Text;

namespace SliceCloud.Service.Utils;

public static class PasswordUtils
{
    #region HashPassword

    public static string HashPassword(string password)
    {
        return Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password))
        );
    }

    #endregion
}
