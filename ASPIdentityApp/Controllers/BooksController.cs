using ASPIdentityApp.Areas.Identity.Data;
using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .ToListAsync();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        //[Authorize(Roles = "Admin,Librarian")]
        //public IActionResult Create()
        //{
        //    ViewBag.Authors = _context.Authors.ToList();
        //    ViewBag.Publishers = _context.Publishers.ToList();
        //    return View();
        //}

        // GET: Books/Create
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return View();
        }

        // Helper method to populate ViewBags
        private async Task PopulateViewBags()
        {
            try
            {
                // Get authors and convert to SelectList
                var authors = await _context.Authors.ToListAsync();
                ViewBag.Authors = new SelectList(authors, "Id", "FullName");

                // Get publishers and convert to SelectList
                var publishers = await _context.Publishers.ToListAsync();
                ViewBag.Publishers = new SelectList(publishers, "Id", "Name");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error populating ViewBags: {ex.Message}");
                ViewBag.Authors = new SelectList(new List<Author>(), "Id", "FullName");
                ViewBag.Publishers = new SelectList(new List<Publisher>(), "Id", "Name");
            }
        }

        // GET: Books/Create
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Create()
        //{
        //    try
        //    {
        //        // Get authors and convert to SelectList
        //        var authors = await _context.Authors.ToListAsync();
        //        ViewBag.Authors = new SelectList(authors, "Id", "FullName");

        //        // Get publishers and convert to SelectList
        //        var publishers = await _context.Publishers.ToListAsync();
        //        ViewBag.Publishers = new SelectList(publishers, "Id", "Name");

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error
        //        Console.WriteLine($"Error in Create GET: {ex.Message}");
        //        return View("Error");
        //    }
        //}

        // POST: Books/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Create(Book book)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        book.AvailableCopies = book.TotalCopies;
        //        _context.Add(book);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.Authors = _context.Authors.ToList();
        //    ViewBag.Publishers = _context.Publishers.ToList();
        //    return View(book);
        //}

        // POST: Books/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Create(Book book)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            book.AvailableCopies = book.TotalCopies;
        //            book.CreatedDate = DateTime.Now;
        //            _context.Add(book);
        //            await _context.SaveChangesAsync();

        //            TempData["SuccessMessage"] = "Book added successfully!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", "An error occurred while saving the book: " + ex.Message);
        //        }
        //    }

        //    // If we got here, something went wrong - repopulate ViewBags
        //    await PopulateCreateViewBags();
        //    return View(book);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Create(Book book)
        //{
        //    Console.WriteLine("=== BOOK CREATE POST STARTED ===");
        //    Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
        //    Console.WriteLine($"AuthorId: {book.AuthorId}");
        //    Console.WriteLine($"PublisherId: {book.PublisherId}");
        //    Console.WriteLine($"Title: {book.Title}");

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            Console.WriteLine("ModelState is VALID - Saving book...");
        //            book.AvailableCopies = book.TotalCopies;
        //            book.CreatedDate = DateTime.Now;
        //            _context.Add(book);
        //            await _context.SaveChangesAsync();

        //            Console.WriteLine("Book saved successfully!");
        //            TempData["SuccessMessage"] = "Book added successfully!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"ERROR: {ex.Message}");
        //            ModelState.AddModelError("", "An error occurred while saving the book: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("ModelState is INVALID");
        //        foreach (var state in ModelState)
        //        {
        //            foreach (var error in state.Value.Errors)
        //            {
        //                Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
        //            }
        //        }
        //    }

        //    // Repopulate ViewBags
        //    await PopulateViewBags();
        //    return View(book);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Create([Bind("Title,ISBN,AuthorId,PublisherId,PublishedDate,TotalCopies,Genre,Description")] Book book)
        //{
        //    Console.WriteLine($"=== BOOK CREATE POST ===");
        //    Console.WriteLine($"AuthorId from form: {book.AuthorId}");
        //    Console.WriteLine($"PublisherId from form: {book.PublisherId}");
        //    Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

        //    // Manually check the ModelState for AuthorId and PublisherId
        //    if (ModelState.ContainsKey("AuthorId"))
        //    {
        //        Console.WriteLine($"AuthorId ModelState: {ModelState["AuthorId"]?.ValidationState}");
        //        foreach (var error in ModelState["AuthorId"]?.Errors ?? new())
        //        {
        //            Console.WriteLine($"AuthorId Error: {error.ErrorMessage}");
        //        }
        //    }

        //    if (ModelState.ContainsKey("PublisherId"))
        //    {
        //        Console.WriteLine($"PublisherId ModelState: {ModelState["PublisherId"]?.ValidationState}");
        //        foreach (var error in ModelState["PublisherId"]?.Errors ?? new())
        //        {
        //            Console.WriteLine($"PublisherId Error: {error.ErrorMessage}");
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            book.AvailableCopies = book.TotalCopies;
        //            book.CreatedDate = DateTime.Now;
        //            _context.Add(book);
        //            await _context.SaveChangesAsync();

        //            TempData["SuccessMessage"] = $"Book '{book.Title}' added successfully!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Database Error: {ex.Message}");
        //            ModelState.AddModelError("", "An error occurred while saving the book: " + ex.Message);
        //        }
        //    }

        //    await PopulateViewBags();
        //    return View(book);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create(Book book)
        {
            // DEBUG: Check what values are actually coming in
            Console.WriteLine($"=== DEBUG: Book Create ===");
            Console.WriteLine($"AuthorId received: {book.AuthorId}");
            Console.WriteLine($"PublisherId received: {book.PublisherId}");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"ISBN: {book.ISBN}");

            // MANUAL VALIDATION - Skip ModelState validation for these fields temporarily
            ModelState.Remove("Author");
            ModelState.Remove("Publisher");

            // Check if the IDs are actually provided
            if (book.AuthorId == 0)
            {
                ModelState.AddModelError("AuthorId", "The Author field is required.");
            }

            if (book.PublisherId == 0)
            {
                ModelState.AddModelError("PublisherId", "The Publisher field is required.");
            }

            // If we have the required IDs, proceed
            if (book.AuthorId > 0 && book.PublisherId > 0 && ModelState.IsValid)
            {
                try
                {
                    book.AvailableCopies = book.TotalCopies;
                    book.CreatedDate = DateTime.Now;
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Book '{book.Title}' added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            await PopulateViewBags();
            return View(book);
        }

        // Helper method to populate ViewBags for Create
        private async Task PopulateCreateViewBags()
        {
            var authors = await _context.Authors.ToListAsync();
            ViewBag.Authors = new SelectList(authors, "Id", "FullName");

            var publishers = await _context.Publishers.ToListAsync();
            ViewBag.Publishers = new SelectList(publishers, "Id", "Name");
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Use the helper method to populate ViewBags
            await PopulateCreateViewBags();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBook = await _context.Books.FindAsync(id);
                    if (existingBook != null)
                    {
                        // Calculate new available copies if total copies changed
                        var diff = book.TotalCopies - existingBook.TotalCopies;
                        existingBook.AvailableCopies += diff;

                        // Ensure available copies don't go negative
                        if (existingBook.AvailableCopies < 0)
                            existingBook.AvailableCopies = 0;

                        existingBook.Title = book.Title;
                        existingBook.ISBN = book.ISBN;
                        existingBook.AuthorId = book.AuthorId;
                        existingBook.PublisherId = book.PublisherId;
                        existingBook.PublishedDate = book.PublishedDate;
                        existingBook.TotalCopies = book.TotalCopies;
                        existingBook.Description = book.Description;
                        existingBook.Genre = book.Genre;

                        _context.Update(existingBook);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Book updated successfully!";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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

            // If model state is invalid, repopulate ViewBags
            await PopulateCreateViewBags();
            return View(book);
        }

        // GET: Books/Edit/5
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var book = await _context.Books.FindAsync(id);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.Authors = _context.Authors.ToList();
        //    ViewBag.Publishers = _context.Publishers.ToList();
        //    return View(book);
        //}

        // POST: Books/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Librarian")]
        //public async Task<IActionResult> Edit(int id, Book book)
        //{
        //    if (id != book.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var existingBook = await _context.Books.FindAsync(id);
        //            if (existingBook != null)
        //            {
        //                // Calculate new available copies if total copies changed
        //                var diff = book.TotalCopies - existingBook.TotalCopies;
        //                existingBook.AvailableCopies += diff;

        //                existingBook.Title = book.Title;
        //                existingBook.ISBN = book.ISBN;
        //                existingBook.AuthorId = book.AuthorId;
        //                existingBook.PublisherId = book.PublisherId;
        //                existingBook.PublishedDate = book.PublishedDate;
        //                existingBook.TotalCopies = book.TotalCopies;
        //                existingBook.Description = book.Description;
        //                existingBook.Genre = book.Genre;

        //                _context.Update(existingBook);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BookExists(book.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.Authors = _context.Authors.ToList();
        //    ViewBag.Publishers = _context.Publishers.ToList();
        //    return View(book);
        //}

        // GET: Books/Delete/5


        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
        // Simple action to check if data is seeded
        public async Task<IActionResult> CheckData()
        {
            var authorCount = await _context.Authors.CountAsync();
            var publisherCount = await _context.Publishers.CountAsync();

            return Content($"Authors: {authorCount}, Publishers: {publisherCount}");
        }

        // Add this method to your existing BooksController
        [Authorize]
        public async Task<IActionResult> Browse()
        {
            var availableBooks = await _context.Books
                .Where(b => b.AvailableCopies > 0)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .ToListAsync();

            return View(availableBooks);
        }
    }
}
