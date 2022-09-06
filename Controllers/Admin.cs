using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectionsPortal.Controllers
{
    [Authorize(Policy = "RequireAdmin")]
    public class Admin : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public Admin(ApplicationDbContext db, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users(int currentPageIndex = 1)
        {
            try
            {
                var usersView = new UserViewModel
                {
                    UsersPerPage = 10,
                    Users = _context.Users.OrderBy(u => u.Id),
                    CurrentPage = currentPageIndex
                };

                return View(usersView);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }


        }


        public async Task<IActionResult> MakeAdmin(string email)
        {
            try
            {
                if (email != null)
                {
                    User user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        if (user.isAdmin)
                        {

                            await _userManager.RemoveFromRoleAsync(user, "Administrator");
                            user.isAdmin = false;
                            if (user.UserName == User.Identity.Name)
                            {
                                //need to revalidate session somehow, cause RemoveFromRole doesnt invalidate session
                                await _signInManager.SignOutAsync();
                                return RedirectToAction("Index", "HomeController");

                            }
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "Administrator");
                            user.isAdmin = true;
                        }
                        _context.SaveChanges();
                    }

                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }

        }

        public async Task<IActionResult> Block(string email)
        {
            try
            {
                if (email != null)
                {
                    User user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        if (user.LockoutEnd == null)
                        {
                            user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
                            if (user.UserName == User.Identity.Name)
                            {
                                await _signInManager.SignOutAsync();
                            }

                        }
                        else
                        {
                            user.LockoutEnd = null;
                        }
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }


        }

        public async Task<IActionResult> Delete(string email)
        {
            try
            {
                if (email != null)
                {
                    User user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        await _userManager.DeleteAsync(user);

                        if (user.UserName == User.Identity.Name)
                        {
                            await _signInManager.SignOutAsync();
                        }
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }

        }

    }
}
