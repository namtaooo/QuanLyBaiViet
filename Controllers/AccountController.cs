using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using QuanLyBaiViet.Data;
using QuanLyBaiViet.Models;

namespace QuanLyBaiViet.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)//inject để controller có thể truy vấn và lưu dữ liệu
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FullName,BirthDay,Email,UserName,Password")] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            user.Role = RoleStatus.Author;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "Đăng ký thành công";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var user = _context.Users.SingleOrDefault
                (u => u.UserName == login.UserName && u.Password == login.Password );

            if (user == null)
            {
                ModelState.AddModelError("","Tài khoản và mật khẩu không hợp lệ.");
                return View(login);
            }
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            
            TempData["AlertMessage"] = "Đăng nhập thành công";
            
            return RedirectToAction("index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index","Home");
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()=> View();
    }
}
