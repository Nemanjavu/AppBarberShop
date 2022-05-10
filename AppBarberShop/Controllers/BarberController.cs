using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppBarberShop.Controllers
{
    [Authorize]
    public class BarberController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private BookingContext db = new BookingContext();
        

        
        public BarberController(ApplicationDbContext context)
        {
            _context = context;
        }
      


        // GET: Barbers
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            
            var barbers = from r in _context.Barbers
                        select r;

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
        
            Barber barbers = _context.Barbers.Where(n => n.BarberName == keyword).FirstOrDefault();
            if (barbers == null)
            {
                return NotFound();
            }
            return View(barbers);
        }

        // GET: MeetingRooms/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: MeetingRooms/Create
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
                        ModelState.AddModelError("", "A meeting room with the same name already exists.");
                    }
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(barber);
        }

        // GET: MeetingRooms/Edit/5
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

        // POST: MeetingRooms/Edit/5
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

            //if (TryUpdateModel(barberToUpdate, "", new string[] { "BarberName" }))
            if (ModelState.IsValid)
                {
                try
                {
                    //check that a room with same name does not already exists if name is being changed
                    string newName = barberToUpdate.BarberName;
                    if (newName != oldName && checkSet.Contains(newName))
                    {
                        ModelState.AddModelError("", "A meeting room with the same name already exists.");
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

        // GET: MeetingRooms/Delete/5
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

        // POST: MeetingRooms/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Barber barberToDelete = _context.Barbers.Find(id);
                //check if any meeting is happening in the room at a future date
                Booking bookingUsingRoomInFuture = _context.Bookings.FirstOrDefault(x => (x.BarberId == id) && (x.Date >= DateTime.Today));
                if (bookingUsingRoomInFuture == null)
                {
                    _context.Barbers.Remove(barberToDelete);
                    _context.SaveChanges();
                }
                else
                {
                    TempData["msg"] = "<script>alert('This meeting room cannot be deleted as it has been booked for future meetings.');</script>";
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