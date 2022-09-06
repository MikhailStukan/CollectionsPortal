using CollectionsPortal.CloudStorage;
using CollectionsPortal.Data;
using CollectionsPortal.Models;
using CollectionsPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionsPortal.Controllers
{
    public class Collections : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICloudStorage _cloudStorage;

        public Collections(ApplicationDbContext db, UserManager<User> userManager, ICloudStorage cloudStorage)
        {
            _context = db;
            _userManager = userManager;
            _cloudStorage = cloudStorage;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int collectionId)
        {
            try
            {
                var collection = await _context.Collections.Where(p => p.Id == collectionId).Include(p => p.User).Include(p => p.Topic).Include(p => p.FieldTemplates).FirstOrDefaultAsync();

                var items = await _context.Items.Where(p => p.Collection == collection).Include(p => p.Likes).Include(p => p.Comments).Include(p => p.Fields).ToListAsync();
                var tags = await _context.TagsToCollections.Where(p => p.Collection == collection).Select(p => p.Tag).ToListAsync();

                if (collection == null)
                    return NotFound();

                ViewBag.Collection = collection;

                ViewBag.Tags = tags;

                ViewBag.Owner = collection.User;

                ViewBag.Items = items;

                ViewBag.Fields = collection.FieldTemplates;

                return View();
            }
            catch(Exception ex)
            {
                return View("Error", ex.Message);
            }
            
        }


        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(int collectionId)
        {
            try
            {
                var collection = await _context.Collections.Where(p => p.Id == collectionId).FirstOrDefaultAsync();
                ViewBag.Collection = collection;

                return View();
            }
            catch(Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Edit(EditCollectionViewModel model)
        {
            try
            {
                var collection = await _context.Collections.Where(p => p.Id == model.collectionId).Include(p => p.User).FirstOrDefaultAsync();
                if (model.Name == null || model.Description == null)
                {
                    ViewBag.Collection = collection;
                    return View(model);
                }
                else
                {
                    if (collection != null)
                    {
                        if (User.Identity.Name == collection.User.UserName || User.IsInRole("Administrator"))
                        {
                            collection.Name = model.Name;
                            collection.Description = model.Description;
                            collection.UpdatedAt = DateTime.Now;

                            if (model.ImageFile != null)
                            {
                                string fileNameForStorage = $"{collection.User.Id}{collection.Id}{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(model.ImageFile.FileName)}";
                                collection.imageUrl = await _cloudStorage.UploadFileAsync(model.ImageFile, fileNameForStorage);
                            }
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });
                        }
                    }
                }
                return View();
            }
            catch(Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Topics = await _context.Topics.ToListAsync();
            return View();
        }

        [HttpGet("getTags")]
        [Route("api/getTags")]
        public IActionResult GetTags()
        {
            string term = HttpContext.Request.Query["term"].ToString();
            var tags = _context.Tags.Where(p => p.Name.StartsWith(term)).Select(u => u.Name).ToList();
            return Ok(tags);
        }


        [Authorize(Policy = "RequireUser")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCollectionViewModel model)
        {
            try
            {
                if (model.Name == null || model.Description == null || model.Description == null || model.Fields == null || model.Tags == null)
                {
                    //small validation cause ModelState doesnt work on ForeignKeys
                    ViewBag.Topics = await _context.Topics.ToListAsync();
                    return View(model);
                }
                else
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    var topic = await _context.Topics.FirstOrDefaultAsync(p => p.Id == model.Topic.Id);
                    var existingTags = _context.Tags.ToList().Select(u => u.Name);

                    Collection collection = new Collection()
                    {
                        User = user,
                        Name = model.Name,
                        Description = model.Description,
                        FieldTemplates = model.Fields,
                        Topic = topic,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    if (model.ImageFile != null)
                    {
                        string fileNameForStorage = $"{collection.User.Id}{collection.Id}{DateTime.Now.ToString("yyyyMMddHHmmss")}{Path.GetExtension(model.ImageFile.FileName)}";
                        collection.imageUrl = await _cloudStorage.UploadFileAsync(model.ImageFile, fileNameForStorage);
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
                            TagsToCollection tagsTo = new TagsToCollection()
                            {
                                Collection = collection,
                                Tag = newTag
                            };
                            await _context.Tags.AddAsync(newTag);
                            await _context.TagsToCollections.AddAsync(tagsTo);
                        }
                        else
                        {
                            TagsToCollection tagsTo = new TagsToCollection()
                            {
                                Collection = collection,
                                Tag = _context.Tags.Where(p => p.Name == tag).FirstOrDefault()
                            };
                            await _context.TagsToCollections.AddAsync(tagsTo);
                        }
                    }
                    await _context.AddAsync(collection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Collections", new { collectionId = collection.Id });
                }
            }
            catch(Exception e)
            {
                return View("Error", e.Message);
            }
        }

        [Authorize(Policy = "RequireUser")]
        public async Task<IActionResult> Delete(int collectionId)
        {
            try
            {
                var coll = await _context.Collections.Where(p => p.Id == collectionId).Include(p => p.User).FirstOrDefaultAsync();

                if (coll == null)
                    return NotFound();

                if (User.Identity.Name != null && (User.Identity.Name == coll.User.UserName || User.IsInRole("Administrator")))
                {
                    if (coll.imageUrl != null)
                    {
                        Uri uri = new Uri(coll.imageUrl);
                        string fileName = uri.Segments.LastOrDefault();
                        await _cloudStorage.DeleteFileAsync(fileName);
                    }

                    _context.Collections.Remove(coll);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Profile");
            }
            catch(Exception e)
            {
                return View("Error", e.Message);
            }
        }

    }
}

