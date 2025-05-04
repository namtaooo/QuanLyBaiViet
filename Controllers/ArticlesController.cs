using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBaiViet.Data;
using QuanLyBaiViet.Models;

namespace QuanLyBaiViet.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(new List<Article>());
            }
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userid,out var ui))
            {
                return Forbid();
            }
            var applicationDbContext = _context.Articles.Include(a => a.Author).Include(a => a.Topic).Where(a => a.AuthorId == ui);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "Email");
            ViewData["TopicID"] = new SelectList(_context.Topics, "TopicId", "TopicName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleModel am)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out var userid))
                {
                    return Forbid();
                }
                var article = new Article()
                {
                    Title = am.Title,
                    Summary = am.Summary,
                    Content = am.Content,
                    TopicID = am.TopicID,
                    AuthorId = userid,
                    SubmitedDate = DateTime.Now,
                    Status = ArticleStatus.Pending
                };

                
                _context.Add(article);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Tạo bài viết thành công";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(Index));
            }
            ViewData["TopicID"] = new SelectList(_context.Topics, "TopicId", "TopicName", am.TopicID);
            TempData["AlertMessage"] = "Tạo bài viết thất bại";
            TempData["AlertType"] = "danger";
            return View(am);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "Email", article.AuthorId);
            ViewData["TopicID"] = new SelectList(_context.Topics, "TopicId", "TopicName", article.TopicID);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Title,AuthorId,SubmitedDate,Summary,Content,TopicID,Status")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "Email", article.AuthorId);
            ViewData["TopicID"] = new SelectList(_context.Topics, "TopicId", "TopicName", article.TopicID);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DashBoard()
        {

            var authorStat = await _context.Articles
                .GroupBy(a => a.AuthorId).Select(g => new AuthorStat
                {
                    AuthorName = g.First().Author.FullName,
                    ArticleCount = g.Count()
                }).ToListAsync();
            var topicStat = await _context.Topics.Select(g => new TopicStat
            {
                TopicName = g.TopicName,
                TopicCount = g.Articles.Count()
            }).ToListAsync();

            var statusCount = await _context.Articles.GroupBy(a => a.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
            var statusStat = new StatusStat
            {
                Pending = statusCount.FirstOrDefault(x => x.Status == ArticleStatus.Pending)?.Count ?? 0,
                Approved = statusCount.FirstOrDefault(x => x.Status == ArticleStatus.Approved)?.Count ?? 0,
                Rejected = statusCount.FirstOrDefault(x => x.Status == ArticleStatus.Rejected)?.Count ?? 0
            };
            var vm = new ViewModels
            {
                AuthorStats = authorStat,
                TopicStats = topicStat,
                StatusStats = statusStat
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pendingList = await _context.Articles.Include(a => a.Author).Include(a => a.Topic)
                .Where(T => T.Status == ArticleStatus.Pending).ToListAsync();
            return View("Pending", pendingList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();
            article.Status = ArticleStatus.Approved;
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Phê duyệt thành công";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();
            article.Status = ArticleStatus.Rejected;
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Từ chối bài viết thành công";
            TempData["AlertType"] = "danger";
            return RedirectToAction(nameof(Pending));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> All()
        {
            var allContext = await _context.Articles.Include(a => a.Author).Include(a => a.Topic).ToListAsync();
            return View("All",allContext);
        }


    }
}
