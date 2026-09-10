using ASPIdentityApp.Areas.Identity.Data;
using ASPIdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASPIdentityApp.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Members
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.ToListAsync();
            return View(members);
        }

        // GET: Members/Details/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Member ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _context.Members
                .Include(m => m.BookIssues)
                    .ThenInclude(bi => bi.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // GET: Members/Create
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            // Set default membership expiry to 1 year from now
            var model = new Member
            {
                MembershipExpiry = DateTime.Now.AddYears(1),
                Status = "Active"
            };

            return View(model);
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Create(Member member)
        {
            // Remove navigation property validation
            ModelState.Remove("BookIssues");

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email already exists
                    var existingMember = await _context.Members
                        .FirstOrDefaultAsync(m => m.Email.ToLower() == member.Email.ToLower());

                    if (existingMember != null)
                    {
                        ModelState.AddModelError("Email", "A member with this email already exists.");
                        return View(member);
                    }

                    // Check if member code already exists
                    var existingCode = await _context.Members
                        .FirstOrDefaultAsync(m => m.MemberCode.ToLower() == member.MemberCode.ToLower());

                    if (existingCode != null)
                    {
                        ModelState.AddModelError("MemberCode", "This member code is already in use.");
                        return View(member);
                    }

                    // Set registration date
                    member.RegistrationDate = DateTime.Now;

                    _context.Add(member);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Member {member.FirstName} {member.LastName} created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the member: " + ex.Message);
                }
            }

            return View(member);
        }

        // GET: Members/Edit/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Member ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Edit(int id, Member member)
        {
            if (id != member.Id)
            {
                TempData["ErrorMessage"] = "Member ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            // Remove navigation property validation
            ModelState.Remove("BookIssues");

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email already exists (excluding current member)
                    var existingMember = await _context.Members
                        .FirstOrDefaultAsync(m => m.Email.ToLower() == member.Email.ToLower() && m.Id != id);

                    if (existingMember != null)
                    {
                        ModelState.AddModelError("Email", "A member with this email already exists.");
                        return View(member);
                    }

                    // Check if member code already exists (excluding current member)
                    var existingCode = await _context.Members
                        .FirstOrDefaultAsync(m => m.MemberCode.ToLower() == member.MemberCode.ToLower() && m.Id != id);

                    if (existingCode != null)
                    {
                        ModelState.AddModelError("MemberCode", "This member code is already in use.");
                        return View(member);
                    }

                    _context.Update(member);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Member {member.FirstName} {member.LastName} updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
                    {
                        TempData["ErrorMessage"] = "Member not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the member: " + ex.Message);
                }
            }

            return View(member);
        }

        // GET: Members/Delete/5
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Member ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _context.Members
                .Include(m => m.BookIssues)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if member has active book issues
            var activeIssues = member.BookIssues?.Any(bi => bi.Status == "Issued") ?? false;
            if (activeIssues)
            {
                TempData["WarningMessage"] = "This member has active book issues. Please return all books before deleting the member.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var member = await _context.Members
                    .Include(m => m.BookIssues)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (member != null)
                {
                    // Check if member has active book issues
                    var activeIssues = member.BookIssues?.Any(bi => bi.Status == "Issued") ?? false;
                    if (activeIssues)
                    {
                        TempData["ErrorMessage"] = "Cannot delete member with active book issues.";
                        return RedirectToAction(nameof(Index));
                    }

                    _context.Members.Remove(member);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Member {member.FirstName} {member.LastName} deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Member not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the member: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: MyMembership - For regular users to view their own membership
        [Authorize]
        public async Task<IActionResult> MyMembership()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return View("NoMembership");
                }

                // Find member by email
                var member = await _context.Members
                    .Include(m => m.BookIssues)
                        .ThenInclude(bi => bi.Book)
                    .FirstOrDefaultAsync(m => m.Email.ToLower() == user.Email.ToLower());

                if (member == null)
                {
                    return View("NoMembership");
                }

                return View(member);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MyMembership: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading your membership details.";
                return View("NoMembership");
            }
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}