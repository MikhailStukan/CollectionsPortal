using CollectionsPortal.CloudStorage;
using CollectionsPortal.Data;
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
        private readonly ICloudStorage _cloudStorage;

        public Item(ApplicationDbContext db, ICloudStorage cloudStorage)
        {
            _context = db;
            _cloudStorage = cloudStorage;
        }
        public async Task<IActionResult> Index(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Collection).Include(p => p.Collection.User).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

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
            var collection = await _context.Collections.Include(p => p.User).Include(p => p.Items).Include(p => p.FieldTemplates).FirstOrDefaultAsync(p => p.Id == model.collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            if (model.Name == null || model.Tags == null || model.Fields == null)
            {
                //small model validation
                ViewBag.Collection = collection;
                return View(model);
            }
            else
            {
                var existingTags = _context.Tags.ToList().Select(u => u.Name);

                foreach (var field in model.Fields)
                {
                    field.FieldTemplates = await _context.FieldTemplates.FirstOrDefaultAsync(p => p.Id == field.FieldTemplates.Id);
                }

                var item = new Models.Item()
                {
                    Collection = collection,
                    Name = model.Name,
                    Fields = model.Fields,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                if (model.ImageFile != null)
                {
                    string fileNameForStorage = $"{item.Collection.User.Id}{item.Id}{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(model.ImageFile.FileName)}";
                    item.imageUrl = await _cloudStorage.UploadFileAsync(model.ImageFile, fileNameForStorage);
                }

                var tags = model.Tags.Split(",");

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
                            Tag = await _context.Tags.Where(p => p.Name == tag).FirstOrDefaultAsync()
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


        [HttpGet]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(int itemId)
        {
            await GetItemdata(itemId);
            return View();
        }

        private async Task GetItemdata(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Collection).Include(p => p.Fields).FirstOrDefaultAsync();
            ViewBag.item = item;
            var tags = await _context.TagsToItems.Where(p => p.Item == item).Include(p => p.Tag).Select(p => p.Tag.Name).ToListAsync();
            ViewBag.Tags = string.Join(",", tags);
            var collection = await _context.Collections.Where(p => p.Id == item.Collection.Id).Include(p => p.FieldTemplates).FirstOrDefaultAsync();
            ViewBag.Collection = collection;
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(EditItemViewModel model)
        {
            if (model.Name == null || model.Fields == null)
            {
                await GetItemdata(model.itemId);
                return View(model);
            }
            else
            {
                var item = await _context.Items.Where(p => p.Id == model.itemId).Include(p => p.Collection.User).Include(p => p.Fields).FirstOrDefaultAsync();
                if (item != null)
                {
                    if (User.Identity.Name == item.Collection.User.UserName || User.IsInRole("Administrator"))
                    {
                        item.Name = model.Name;

                        for (var i = 0; i < model.Fields.Count; i++)
                        {
                            item.Fields[i].Value = model.Fields[i].Value;
                        }

                        if (model.ImageFile != null)
                        {
                            string fileNameForStorage = $"{item.Collection.User.Id}{item.Id}{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(model.ImageFile.FileName)}";
                            item.imageUrl = await _cloudStorage.UploadFileAsync(model.ImageFile, fileNameForStorage);
                        }
                        item.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();


                    }

                }
                return RedirectToAction("Index", "Item", new { itemId = item.Id });
            }
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int itemId)
        {
            var item = await _context.Items.Where(p => p.Id == itemId).Include(p => p.Likes).Include(p => p.Collection).Include(p => p.Fields).Include(p => p.Collection.User).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            if (User.Identity.Name == item.Collection.User.UserName || User.IsInRole("Administrator"))
            {
                if (item.imageUrl != null)
                {
                    Uri uri = new Uri(item.imageUrl);
                    string fileName = uri.Segments.LastOrDefault();
                    await _cloudStorage.DeleteFileAsync(fileName);
                }

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
