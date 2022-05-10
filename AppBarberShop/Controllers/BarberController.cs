using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppBarberShop.Controllers
{
    [Authorize]
    public class BarberController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public BarberController(ApplicationDbContext context)
        {
            _context = context;
        }
     

        // GET: Barbers
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var barbers = from b in _context.Barbers
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                barbers = barbers.Where(r => r.BarberName.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    barbers = barbers.OrderByDescending(r => r.BarberName);
                    break;
               
            }
            return View(barbers.ToList());
        }

        // GET: Barbers/Details/{keyword}
        public ActionResult Details(string keyword)
        {
            Barber barber = _context.Barbers.Where(n => n.BarberName == keyword).FirstOrDefault();
            if (barber == null)
            {
                return NotFound();
            }
            return View(barber);
        }

        // GET: Barber/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Barber/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("BarberName")] Barber barber)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Barbers.FirstOrDefault(m => m.BarberName == barber.BarberName) == null)
                    {
                        _context.Barbers.Add(barber);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "A Barber with the same name already exists.");
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(barber);
        }

        // GET: Barber/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundResult();
            }
            Barber barber = _context.Barbers.Find(id);
            if (barber == null)
            {
                return BadRequest();
            }
            return View(barber);
        }

        // POST: Barber/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new NotFoundResult();
            }

            var barberToUpdate = _context.Barbers.Find(id);
            string oldName = barberToUpdate.BarberName;
            List<string> checkSet = _context.Barbers.Select(m => m.BarberName).ToList();

            if (ModelState.IsValid)
                {
                try
                {
                    //check that a barber with same name does not already exists if name is being changed
                    string newName = barberToUpdate.BarberName;
                    if (newName != oldName && checkSet.Contains(newName))
                    {
                        ModelState.AddModelError("", "A barber with the same name already exists.");
                    }
                    else
                    {
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Please try again, and if the problem persists, contact your system administrator.");
                }
            }
            return View(barberToUpdate);
        }

        // GET: barber/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new NotFoundResult();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again, and if the problem persists, contact your system administrator.";
            }
            Barber barber = _context.Barbers.Find(id);
            if (barber == null)
            {
                return BadRequest();
            }
            return View(barber);
        }

        // POST: Barber/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Barber barberToDelete = _context.Barbers.Find(id);

                Booking bookingUsingBarberInFuture = _context.Bookings.FirstOrDefault(x => (x.BarberId == id) && (x.Date >= DateTime.Today));
                if (bookingUsingBarberInFuture == null)
                {
                    _context.Barbers.Remove(barberToDelete);
                    _context.SaveChanges();
                }
                else
                {
                    TempData["msg"] = "<script>alert('This barber cannot be deleted as it has been booked for future bookings.');</script>";
                }

            }
            catch (RetryLimitExceededException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}