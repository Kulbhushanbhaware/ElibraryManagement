using ASPIdentityApp.Areas.Identity.Data;
using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASPIdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = new DashboardViewModel
                {
                    TotalBooks = await _context.Books.CountAsync(),
                    TotalAuthors = await _context.Authors.CountAsync(),
                    TotalMembers = await _context.Members.CountAsync(),
                    IssuedBooks = await _context.BookIssues.CountAsync(bi => bi.Status == "Issued"),
                    RecentBooks = await _context.Books
                        .Include(b => b.Author)
                        .OrderByDescending(b => b.CreatedDate)
                        .Take(5)
                        .ToListAsync()
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                // Return empty dashboard if database tables don't exist yet
                return View(new DashboardViewModel());
            }
        }
        public async Task<IActionResult> CreateTestData()
        {
            var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();

            // Create test author
            var author = new Author
            {
                FirstName = "Test",
                LastName = "Author",
                Email = "author@test.com",
                DateOfBirth = new DateTime(1980, 1, 1)
            };
            context.Authors.Add(author);

            // Create test publisher
            var publisher = new Publisher
            {
                Name = "Test Publisher",
                Address = "123 Test Street",
                Email = "publisher@test.com",
                Phone = "123-456-7890"
            };
            context.Publishers.Add(publisher);

            await context.SaveChangesAsync();

            // Create test book
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "1234567890123",
                AuthorId = author.Id,
                PublisherId = publisher.Id,
                PublishedDate = new DateTime(2020, 1, 1),
                TotalCopies = 5,
                AvailableCopies = 5,
                Genre = "Fiction",
                Description = "A test book for development"
            };
            context.Books.Add(book);

            await context.SaveChangesAsync();

            return Content("Test data created successfully!");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
    }
    
}
