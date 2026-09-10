using ASPIdentityApp.Areas.Identity.Data;
using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Controllers
{
    public class BookIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookIssuesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BookIssues
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Index()
        {
            var bookIssues = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Member)
                .ToListAsync();
            return View(bookIssues);
        }

        // GET: BookIssues/Issue
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Issue()
        {
            try
            {
                await PopulateViewBags();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Issue GET: {ex.Message}");
                return View("Error");
            }
        }

        // POST: BookIssues/Issue
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Issue(BookIssue bookIssue)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var book = await _context.Books.FindAsync(bookIssue.BookId);
                    if (book == null || book.AvailableCopies <= 0)
                    {
                        ModelState.AddModelError("", "Book is not available for issue.");
                        await PopulateViewBags();
                        return View(bookIssue);
                    }

                    // Check if member already has this book issued
                    var existingIssue = await _context.BookIssues
                        .FirstOrDefaultAsync(bi => bi.BookId == bookIssue.BookId &&
                                                 bi.MemberId == bookIssue.MemberId &&
                                                 bi.Status == "Issued");
                    if (existingIssue != null)
                    {
                        ModelState.AddModelError("", "This book is already issued to this member.");
                        await PopulateViewBags();
                        return View(bookIssue);
                    }

                    // Check if member has reached maximum books limit (3 books)
                    var memberIssuedBooks = await _context.BookIssues
                        .CountAsync(bi => bi.MemberId == bookIssue.MemberId && bi.Status == "Issued");

                    if (memberIssuedBooks >= 3)
                    {
                        ModelState.AddModelError("", "This member has already reached the maximum limit of 3 books.");
                        await PopulateViewBags();
                        return View(bookIssue);
                    }

                    bookIssue.IssueDate = DateTime.Now;
                    bookIssue.DueDate = DateTime.Now.AddDays(14); // 2 weeks
                    bookIssue.Status = "Issued";
                    bookIssue.FineAmount = 0;

                    // Update book available copies
                    book.AvailableCopies--;

                    _context.Add(bookIssue);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Book issued successfully!";
                    return RedirectToAction(nameof(Index));
                }

                await PopulateViewBags();
                return View(bookIssue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Issue POST: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while issuing the book.");
                await PopulateViewBags();
                return View(bookIssue);
            }
        }

        // Helper method to populate ViewBags - UPDATED
        private async Task PopulateViewBags()
        {
            var availableBooks = await _context.Books
                .Where(b => b.AvailableCopies > 0)
                .Include(b => b.Author)
                .ToListAsync();

            var activeMembers = await _context.Members
                .Where(m => m.Status == "Active")
                .ToListAsync();

            ViewBag.Books = new SelectList(availableBooks, "Id", "Title");
            ViewBag.Members = new SelectList(activeMembers, "Id", "MemberCode"); // Changed from "MemberId" to "MemberCode"
        }



        // GET: BookIssues/Return/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bookIssue == null)
            {
                return NotFound();
            }

            // Calculate fine if overdue
            if (bookIssue.DueDate < DateTime.Now && bookIssue.ReturnDate == null)
            {
                var overdueDays = (DateTime.Now - bookIssue.DueDate).Days;
                bookIssue.FineAmount = overdueDays * 5; // $5 per day fine
            }

            return View(bookIssue);
        }
        // POST: BookIssues/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Return(int id)
        {
            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (bookIssue == null)
            {
                return NotFound();
            }

            bookIssue.ReturnDate = DateTime.Now;
            bookIssue.Status = "Returned";

            // Update book available copies
            if (bookIssue.Book != null)
            {
                bookIssue.Book.AvailableCopies++;
            }

            // Calculate final fine if not already set
            if (bookIssue.DueDate < DateTime.Now && bookIssue.FineAmount == 0)
            {
                var overdueDays = (DateTime.Now - bookIssue.DueDate).Days;
                bookIssue.FineAmount = overdueDays * 5; // $5 per day fine
            }

            _context.Update(bookIssue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: MyIssues - For members to view their own issued books - UPDATED
        public async Task<IActionResult> MyIssues()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            // Since we removed UserId from Member, we need to find member by email or other means
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("NoMembership");
            }

            // Find member by email (since we removed UserId relationship)
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == user.Email);

            if (member == null)
            {
                return View("NoMembership");
            }

            var myIssues = await _context.BookIssues
                .Where(bi => bi.MemberId == member.Id)
                .Include(bi => bi.Book)
                    .ThenInclude(b => b.Author)
                .Include(bi => bi.Book)
                    .ThenInclude(b => b.Publisher)
                .OrderByDescending(bi => bi.IssueDate)
                .ToListAsync();

            return View(myIssues);
        }

        // GET: BookIssues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Book)
                    .ThenInclude(b => b.Author)
                .Include(bi => bi.Book)
                    .ThenInclude(b => b.Publisher)
                .Include(bi => bi.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bookIssue == null)
            {
                return NotFound();
            }

            return View(bookIssue);
        }

        private bool BookIssueExists(int id)
        {
            return _context.BookIssues.Any(e => e.Id == id);
        }

        // A simple action to demonstrate a basic view
        public IActionResult SimpleIssue()
        {
            return View();
        }

        // Add this method to your existing BookIssuesController
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SelfIssue(int bookId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Browse", "Books");
                }

                // Find member by email
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Email.ToLower() == user.Email.ToLower());

                if (member == null)
                {
                    TempData["ErrorMessage"] = "You don't have an active membership. Please contact administrator.";
                    return RedirectToAction("Browse", "Books");
                }

                if (member.Status != "Active")
                {
                    TempData["ErrorMessage"] = $"Your membership is {member.Status}. Please contact administrator.";
                    return RedirectToAction("Browse", "Books");
                }

                // Check if membership is expired
                if (member.MembershipExpiry < DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Your membership has expired. Please renew to issue books.";
                    return RedirectToAction("Browse", "Books");
                }

                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.AvailableCopies <= 0)
                {
                    TempData["ErrorMessage"] = "Book is not available for issue.";
                    return RedirectToAction("Browse", "Books");
                }

                // Check if member already has this book issued
                var existingIssue = await _context.BookIssues
                    .FirstOrDefaultAsync(bi => bi.BookId == bookId &&
                                             bi.MemberId == member.Id &&
                                             bi.Status == "Issued");

                if (existingIssue != null)
                {
                    TempData["ErrorMessage"] = "You already have this book issued.";
                    return RedirectToAction("Browse", "Books");
                }

                // Check if member has reached maximum books limit (3 books)
                var memberIssuedBooks = await _context.BookIssues
                    .CountAsync(bi => bi.MemberId == member.Id && bi.Status == "Issued");

                if (memberIssuedBooks >= 3)
                {
                    TempData["ErrorMessage"] = "You have reached the maximum limit of 3 books. Please return some books first.";
                    return RedirectToAction("Browse", "Books");
                }

                // Create book issue
                var bookIssue = new BookIssue
                {
                    BookId = bookId,
                    MemberId = member.Id,
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14), // 2 weeks
                    Status = "Issued",
                    FineAmount = 0,
                    Notes = "Self-issued by member"
                };

                // Update book available copies
                book.AvailableCopies--;

                _context.BookIssues.Add(bookIssue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Book '{book.Title}' issued successfully! Due date: {bookIssue.DueDate:MMM dd, yyyy}";
                return RedirectToAction("MyIssues", "BookIssues");
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in SelfIssue: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while issuing the book. Please try again.";
                return RedirectToAction("Browse", "Books");
            }
        }
    }
}
