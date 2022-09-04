﻿using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionsPortal.Controllers
{

    public class Item : Controller
    {
        private readonly ApplicationDbContext _context;

        public Item(ApplicationDbContext db)
        {
            _context = db;
        }
        public async Task<IActionResult> Index(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Collection).Include(p => p.Collection.User).FirstOrDefaultAsync();

            var fields = _context.Fields.Where(p => p.Item.Id == itemId).Include(p => p.FieldTemplates).ToList();

            var comments = _context.Comments.Where(p => p.Item == item).Include(p => p.User).ToList();

            var likes = _context.Likes.Where(p => p.Item == item).Include(p => p.User).ToList();


            if (User.Identity.Name != null)
            {
                ViewBag.currentUser = await _context.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefaultAsync();
                foreach (var like in likes)
                {
                    if (like.User.UserName == User.Identity.Name)
                    {
                        ViewBag.isLiked = true;
                    }
                }
            }

            ViewBag.Fields = fields;
            ViewBag.Item = item;
            ViewBag.Comments = comments;
            ViewBag.Likes = likes;

            return View();
        }

        [HttpGet]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create(int collectionId)
        {
            var collection = await _context.Collections.Include(p => p.User).Include(p => p.Items).Include(p => p.FieldTemplates).Where(c => c.Id == collectionId).FirstOrDefaultAsync();

            if (collection == null)
            {
                return NotFound();
            }

            if (User.Identity.Name == collection.User.UserName || User.IsInRole("Administrator"))
            {
                ViewBag.Collection = collection;
            }
            else
            {
                return RedirectToAction("Index", "HomeController");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            var collection = _context.Collections.FirstOrDefault(p => p.Id == model.collectionId);

            if (model.Name == null || model.Description == null || model.Tags == null || model.Fields == null)
            {
                //small model validation
                ViewBag.Collection = collection;
                return RedirectToAction("Create", new { collectionId = collection.Id });
            }
            else
            {
                var existingTags = _context.Tags.ToList().Select(u => u.Name);

                var tags = model.Tags.Split(",");

                foreach (var field in model.Fields)
                {
                    field.FieldTemplates = _context.FieldTemplates.FirstOrDefault(p => p.Id == field.FieldTemplates.Id);
                }

                var item = new Models.Item()
                {
                    Collection = collection,
                    Name = model.Name,
                    Description = model.Description,
                    Fields = model.Fields,
                    CreatedAt = DateTime.Now
                };

                foreach (var tag in tags)
                {
                    if (!existingTags.Contains(tag))
                    {
                        Tag newTag = new Tag()
                        {
                            Name = tag
                        };
                        TagsToItems tagsTo = new TagsToItems()
                        {
                            Item = item,
                            Tag = newTag
                        };
                        await _context.Tags.AddAsync(newTag);
                        await _context.TagsToItems.AddAsync(tagsTo);
                    }
                    else
                    {
                        TagsToItems tagsTo = new TagsToItems()
                        {
                            Item = item,
                            Tag = _context.Tags.Where(p => p.Name == tag).FirstOrDefault()
                        };
                        await _context.TagsToItems.AddAsync(tagsTo);
                    }
                }

                collection.UpdatedAt = DateTime.Now;

                await _context.AddAsync(item);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Collections", new { collectionId = model.collectionId });
            }
        }


        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Collection).Include(p => p.Fields).FirstOrDefaultAsync();
            ViewBag.item = item;
            return View();
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(EditItemViewModel model)
        {
            return View();
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Likes).Include(p => p.Collection).Include(p => p.Fields).Include(p => p.Collection.User).FirstOrDefaultAsync();

            if (User.Identity.Name == item.Collection.User.UserName || User.IsInRole("Administrator"))
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Collections", new { collectionId = item.Collection.Id });
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Comment(Comment comment, int itemId)
        {
            if (comment != null)
            {
                comment.User = await _context.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefaultAsync();
                comment.Item = await _context.Items.Where(p => p.Id == itemId).FirstOrDefaultAsync();
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Item", new { itemId = itemId });
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Like(int itemId)
        {
            var user = await _context.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefaultAsync();
            var item = await _context.Items.Where(p => p.Id == itemId).FirstOrDefaultAsync();

            if (user != null && item != null)
            {
                Like like = new Like()
                {
                    User = user,
                    Item = item
                };

                await _context.Likes.AddAsync(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Item", new { itemId = itemId });
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Unlike(int itemId)
        {
            var user = await _context.Users.Where(p => p.UserName == User.Identity.Name).FirstOrDefaultAsync();
            var item = await _context.Items.Where(p => p.Id == itemId).FirstOrDefaultAsync();

            if (user != null && item != null)
            {
                var like = await _context.Likes.Where(p => p.User == user && p.Item == item).FirstOrDefaultAsync();

                if (like != null)
                {
                    _context.Likes.Remove(like);
                    await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction("Index", "Item", new { itemId = itemId });
        }
    }
}
