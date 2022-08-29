﻿using CollectionsPortal.Data;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CollectionsPortal.Models;
using Microsoft.EntityFrameworkCore;

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
            var usersView = new UserViewModel
            {
                UsersPerPage = 10,
                Users = _context.Users.OrderBy(u => u.Id),
                CurrentPage = currentPageIndex
            };

            return View(usersView);
        }


        public async Task<IActionResult> MakeAdmin(string email)
        {
            if (email != null)
            {
                User user = await _userManager.FindByEmailAsync(email);
                
                if(user != null)
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

        public async Task<IActionResult> Block(string email)
        {

            if (email != null)
            {
                User user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    if(user.LockoutEnd == null)
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

        public async Task<IActionResult> Delete(string email)
        {
            if (email != null)
            {
                User user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var items = _context.Collections.OrderBy(p => p.Id).Include(p => p.Items).ToList();
                    foreach (var item in items)
                    {
                        _context.Remove(item);
                    }

                    await _userManager.DeleteAsync(user);


                    if(user.UserName == User.Identity.Name)
                    {
                        await _signInManager.SignOutAsync();
                    }
                    _context.SaveChanges();
                }
            }
         return RedirectToAction("Users");
        }

    }
}
