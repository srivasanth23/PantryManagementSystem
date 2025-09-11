using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryManagementSystem.Models.Domain;
using PantryManagementSystem.Repositories.Interfaces;
using Rotativa.AspNetCore;

namespace PantryManagementSystem.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IBillingRepository repo;
        private readonly UserManager<IdentityUser> _userManager;

        public BillingController(IBillingRepository repo, UserManager<IdentityUser> userManager)
        {
            this.repo = repo;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            var list = await repo.GetAllAsync();
            return View(list);
        }

        [Authorize(Roles = "Admin,Staff,User")]
        public async Task<IActionResult> Details(Guid id)
        {
            var bill = await repo.GetByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        [Authorize(Roles = "Admin,Staff,User")]
        public async Task<IActionResult> DownloadPdf(Guid id)
        {
            var bill = await repo.GetByIdAsync(id);
            if (bill == null)
                return NotFound();

            // Define file name & path
            var fileName = $"Bill_{bill.Month}_{bill.UserEmail}.pdf";
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PdfBills", fileName);

            // Ensure folder exists
            var folder = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Generate PDF as byte array
            var pdfResult = new Rotativa.AspNetCore.ViewAsPdf("BillPdf", bill)
            {
                FileName = fileName,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };

            var pdfBytes = await pdfResult.BuildFile(ControllerContext);

            // Save to folder
            await System.IO.File.WriteAllBytesAsync(savePath, pdfBytes);

            // Return the PDF to browser
            return File(pdfBytes, "application/pdf", fileName);
        }



        //[Authorize(Roles = "User,Staff,Admin")]
        //public async Task<IActionResult> UserBills(Guid userId)
        //{
        //    var list = await repo.GetByUserAsync(userId);
        //    ViewBag.UserId = userId;
        //    return View(list);
        //}
        [Authorize(Roles = "User,Staff,Admin")]
        public async Task<IActionResult> UserBills(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
            {
                // use the logged-in user's Id
                var identityUser = await _userManager.GetUserAsync(User);
                if (identityUser == null)
                    return Unauthorized();

                userId = Guid.Parse(identityUser.Id);
            }

            var list = await repo.GetByUserAsync(userId.Value);
            ViewBag.UserId = userId.Value;
            return View(list);
        }


        [Authorize(Roles = "Staff")]
        [HttpGet]
        public IActionResult Generate()
        {
            var users = _userManager.Users
                .Select(u => new { u.Id, u.Email })
                .ToList();
            ViewBag.Users = users;
            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(Guid userId, string month)
        {
            if (userId == Guid.Empty)
            {
                ModelState.AddModelError("", "UserId is required.");
                var users = _userManager.Users.Select(u => new { u.Id, u.Email }).ToList();
                ViewBag.Users = users;
                return View();
            }

            if (string.IsNullOrWhiteSpace(month))
            {
                month = DateTime.UtcNow.ToString("MMM-yyyy");
            }

            var saved = await repo.GenerateForUserMonthAsync(userId, month.Trim());
            TempData["Notice"] = $"Bill generated for {saved.Month}, Total = {saved.TotalAmount}";
            return RedirectToAction(nameof(Details), new { id = saved.Id });
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var bill = await repo.GetByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
