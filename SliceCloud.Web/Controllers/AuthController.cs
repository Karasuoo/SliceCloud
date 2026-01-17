using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SliceCloud.Repository.Models;
using SliceCloud.Repository.ViewModels;
using SliceCloud.Service.Interfaces;
using SliceCloud.Service.Utils;

namespace SliceCloud.Web.Controllers;

public class AuthController(IAuthService authService, IJwtService jwtService, IEmailSenderService emailSenderService) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEmailSenderService _emailSenderService = emailSenderService;

    #region Login GET

    public IActionResult Login()
    {
        try
        {
            (string? Email, string? Username)? user = SessionUtils.GetUser(HttpContext);

            if (user == null)
                return View();


            ClaimsPrincipal? principal = null;
            string? token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                principal = _jwtService.ValidateToken(token);
            }

            if (principal == null)
            {
                Response.Cookies.Delete("AuthToken");
                CookieUtils.ClearCookies(HttpContext);
                SessionUtils.ClearSession(HttpContext);
                return View();
            }
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
            return View();
        }
    }

    #endregion

    #region Login POST

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            UsersLogin? usersLogin = await _authService.AuthenticateUser(
                         loginViewModel.Email!.ToLower(),
                         loginViewModel.Password!
                     );

            if (usersLogin is null)
            {
                bool userExists = await _authService.CheckIfUserExists(loginViewModel.Email);
                UsersLogin? userReset = await _authService.GetUserLoginByEmailAsync(loginViewModel.Email.ToLower());
                if (userReset is not null && userReset.IsFirstLogin)
                {
                    string resetToken = await _authService.GeneratePasswordResetToken(
                        userReset.Email!
                    );

                    return RedirectToAction(
                        "ResetPassword",
                        "Auth",
                        new { token = resetToken }
                    );
                }
                if (userExists)
                {
                    ModelState.AddModelError(
                        "Password",
                        "Invalid password. Try again or reset your password."
                    );
                }
                return View(loginViewModel);
            }
            string token = await _jwtService.GenerateJwtToken(loginViewModel.Email, loginViewModel.RememberMe);
            CookieUtils.SaveJWTToken(Response, token);
            if (loginViewModel.RememberMe)
            {
                CookieUtils.SaveUserData(Response, usersLogin);
            }
            HttpContext.Session.SetString("username", usersLogin.User!.UserName!);
            return RedirectToAction("Dashboard", "Dashboard");
        }
        catch
        {
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
            return View("Error");
        }
    }

    #endregion

    #region ForgotPassword GET

    [HttpGet]
    public IActionResult ForgotPassword(string email = "")
    {
        try
        {
            return View(new ForgotPasswordViewModel { Email = email });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
            return View("Error");
        }
    }

    #endregion

    #region ForgotPassword POST

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool userExists = await _authService.CheckIfUserExists(model.Email);
            if (!userExists)
            {
                ModelState.AddModelError("Email", "No account found with this email.");
                return View(model);
            }

            string resetToken = await _authService.GeneratePasswordResetToken(model.Email);
            string? resetLink = Url.Action(
                "ResetPassword",
                "Auth",
                new { token = resetToken },
                Request.Scheme
            );

            await _emailSenderService.SendResetPasswordEmail(model.Email, resetLink);

            TempData["SuccessMessage"] = "A password reset link has been sent to your email.";
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to send reset email.";
            return View(model);
        }
    }

    #endregion

    #region ResetPassword GET

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("Login");
            }

            bool isValid = await _authService.ValidatePasswordResetToken(token);
            if (!isValid)
            {
                TempData["ErrorMessage"] = "Invalid or expired reset link.";
                return RedirectToAction("Login");
            }

            ResetPasswordViewModel? resetPasswordViewModel = new ResetPasswordViewModel { Token = token };
            TempData["InfoMessage"] = "Please reset you password";

            return View(resetPasswordViewModel);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
            return RedirectToAction("Login");
        }
    }

    #endregion

    #region ResetPassword POST

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = await _authService.ValidatePasswordResetToken(model.Token ?? string.Empty);
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid or expired reset link.");
                return View(model);
            }

            bool result = await _authService.UpdateUserPassword(model.Token ?? string.Empty, model.NewPassword!);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to reset password.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Your password has been successfully reset. Please log in with your new password.";
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred. Please try again.";
            return View(model);
        }
    }

    #endregion

    #region LogOut Method

    public IActionResult Logout()
    {
        try
        {
            CookieUtils.ClearCookies(HttpContext);
            SessionUtils.ClearSession(HttpContext);
            return RedirectToAction("Login", "Auth");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again.";
            return RedirectToAction("Login", "Auth");
        }
    }

    #endregion

    #region RefreshToken 

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            string? oldToken = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(oldToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            ClaimsPrincipal? principal = _jwtService.ValidateToken(oldToken);

            if (principal == null)
                return RedirectToAction("Login", "Auth");

            string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Auth");

            string? newToken = await _jwtService.GenerateJwtToken(email);
            CookieUtils.SaveJWTToken(Response, newToken);

            return Ok(new { success = true });
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while refreshing your session. Please try again.";
            return RedirectToAction("Login", "Auth");
        }
    }

    #endregion
}
