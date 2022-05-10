

using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Models;
using AppBarberShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace AppBarberShop.Controllers
{
    public class BookingController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;

        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor=httpContextAccessor;
        }
        
        public ActionResult Index()
        {
            
            IEnumerable<Booking> bookings;
            if (User.IsInRole("Admin"))
            {
                bookings = _context.Bookings.Where(b => b.Date >= DateTime.Today).Include(b => b.Barber).OrderBy(b => b.Date).ThenBy(b => b.Start_DateTime).ToList();
            }
            else
            {
                var user_id = _httpContextAccessor.HttpContext?.User.GetUserId();

                bookings = _context.Bookings.Where(b => b.UserId == user_id).Where(b => b.Date >= DateTime.Today).Include(b => b.Barber).OrderBy(b => b.Date).ThenBy(b => b.Start_DateTime).ToList();
            }

            return View(bookings);
        }

        // GET: Booking/Create
        //Used to Collect Criteria to Find Barber
        public ActionResult Create()
        {
            PopulateStartTimeDropDownList();
            PopulateEndTimeDropDownList();
            return View();
        }

        // POST: Booking/Create2
        //Used to Pick Barber
        [HttpPost]
        public ActionResult Create2(BookingCreateViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }
            if (vm.InvalidStartAndEnd())
            {
                ModelState.AddModelError("", "Please check start and end times. A booking cannot end before it starts.");
            }

            //Check that the date is valid
            if (vm.InValidDate())
            {
                ModelState.AddModelError("", "Invalid Date.");
            }//Ensure that the time is not prior to now
            if (vm.InvalidStartTime())
            {
                ModelState.AddModelError("", "Please check start time. A booking cannot be made prior to name.");
            }


            if (ModelState.IsValid)
            {
                //find available barbers!
                List<Barber> availableBarbers = FindAvailableBarbers(vm.Date, vm.Start_DateTime, vm.End_DateTime);
                if (availableBarbers.Count == 0)
                {
                    ModelState.AddModelError("", "Sorry, there is no available barber at the specified date and time.");
                }
                else

                {
                    
                    Booking newBooking = new Booking
                    {
                        Date = vm.Date,
                        Start_DateTime = vm.Start_DateTime,
                        End_DateTime = vm.End_DateTime,
                        
                    };

                    ViewBag.Username = _httpContextAccessor.HttpContext?.User.GetUserId();
                    ViewBag.AvailableBarbers = availableBarbers;
                    ViewBag.BarberId = new SelectList(availableBarbers, "BarberId", "BarberName");
                    return View(newBooking);
                }
            }
            PopulateStartTimeDropDownList(vm.Start_DateTime);
            PopulateEndTimeDropDownList(vm.End_DateTime);
            return View("Create", vm);
        }

        // POST: Booking/CreatePost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost([Bind("BarberId, Service, Date, Start_DateTime, End_DateTime, UserId")] Booking booking)
        {
            var user_id = _httpContextAccessor.HttpContext?.User.GetUserId();
            booking.UserId = user_id;

            try
            {

                if (ModelState.IsValid)
                {
                    //make sure barber is still free
                    foreach (Booking b in _context.Bookings)
                    {
                        if (!b.IsValidBooking(booking))
                        {
                            ModelState.AddModelError("", "This barber is not available any longer for booking. Please try to make another booking.");
                            PopulateStartTimeDropDownList(booking.Start_DateTime);
                            PopulateEndTimeDropDownList(booking.End_DateTime);
                            return View("Create", new { Date = booking.Date });
                        }
                    }
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Sorry, there was an error. Please try again.");
                return RedirectToAction("Create");
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again, and if the problem persists, contact your system admistrator");
                return RedirectToAction("Create");
            }
        }


        // GET: Booking/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            Booking booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            //check if the booking is from the user logged in unless he is an admin
            try
            {
                if (booking.UserId == User.Identity.Name)
                {
                    throw new UnauthorizedAccessException("Oops, this booking doesn't seem to be yours, you cannot edit it.");
                }
                //using a viewmodel to pass on data from controller to view then to controller again when posting
                BookingEditViewModel vm = new BookingEditViewModel
                {
                    BookingId = booking.BookingId,
                    Date = booking.Date,
                    Service = booking.Service,
                    Start_DateTime = booking.Start_DateTime,
                    End_DateTime = booking.End_DateTime
                };

                ViewBag.BarberId = new SelectList(_context.Barbers, "BarberId", "BarberName", booking.BarberId);
                PopulateStartTimeDropDownList(booking.Start_DateTime);
                PopulateEndTimeDropDownList(booking.End_DateTime);
                return View(vm);
            }
            catch (UnauthorizedAccessException ex)
            {
                return View("NotAuthorizedError", ex);//, new HandleErrorInfo(ex, "Booking", "Edit"));
            }
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(BookingEditViewModel vm)
        {
            if (vm == null)
            {
                return new BadRequestResult();
            }
            Booking bookingToUpdate = _context.Bookings.Find(vm.BookingId.Value);

            //check valid times
            if (vm.InvalidStartAndEnd())
            {
                ModelState.AddModelError("", "Please check start and end times. A meeting cannot end before it starts.");
            }
            //Ensure that the time is not prior to now
            if (vm.InvalidStartTime())
            {
                ModelState.AddModelError("", "Please check start time. A booking cannot be made prior to name.");
            }

            if (vm.InValidDate())
            {
                ModelState.AddModelError("", "Invalid Date");            
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //create set of existing bookings
                    List<Booking> checkSet = _context.Bookings.ToList();
                    checkSet.Remove(bookingToUpdate);

                    //assign vm values to booking
                    bookingToUpdate.Service = vm.Service;
                    bookingToUpdate.Date = vm.Date;
                    bookingToUpdate.Start_DateTime = vm.Start_DateTime;
                    bookingToUpdate.End_DateTime = vm.End_DateTime;
                    bookingToUpdate.BarberId = vm.BarberId;

                    //make sure barber is free                    
                    foreach (Booking b in checkSet)
                    {
                        if (!b.IsValidBooking(bookingToUpdate))
                        {
                            ModelState.AddModelError("", "The selected barber is not available at the selected date and times. Please try to make another booking.");
                            ViewBag.BarberId = new SelectList(_context.Barbers, "BarberId", "BarberName", bookingToUpdate.BarberId);
                            PopulateStartTimeDropDownList(bookingToUpdate.Start_DateTime);
                            PopulateEndTimeDropDownList(bookingToUpdate.End_DateTime);
                            return View("Edit", vm);
                        }
                    }
                    try
                    {
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. Please try again. If problem persists, contact your system administrator.");
                    }
                }
            }
            ViewBag.BarberId = new SelectList(_context.Barbers, "BarberId", "BarberName", bookingToUpdate.BarberId);
            PopulateStartTimeDropDownList(vm.Start_DateTime);
            PopulateEndTimeDropDownList(vm.End_DateTime);
            return View("Edit", vm);
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Please try again. If problem persists, contact your system administrator.";
    }
            Booking booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
}
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = _context.Bookings.Find(id);
            //check if the booking is from the user logged in ***unless he is an admin***
            try
            {
                if (booking.UserId == User.Identity.Name)
                {
                    throw new UnauthorizedAccessException("Oops, this booking doesn't seem to be yours, you cannot delete it.");
                }
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException ex)
            {
                return View("NotAuthorizedError", ex);//, new HandleErrorInfo(ex, "Booking", "Index"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        //Find Available Barbers when date and time are picked
        private List<Barber> FindAvailableBarbers(DateTime date, DateTime startTime, DateTime endTime)
        {
            List<Barber> availableBarber = _context.Barbers.OrderBy(r => r.BarberName).ToList();

            //find booking on same day
            IEnumerable<Booking> filteredBookings = _context.Bookings.Where(b => b.Date.Year == date.Year && b.Date.Month == date.Month && b.Date.Day == date.Day).ToList();

            //for each booking at same time, eliminate barber
            foreach (Booking item in filteredBookings)
            {
                if (!((item.Start_DateTime > endTime) || (item.End_DateTime < startTime)))
                {
                    availableBarber.Remove(item.Barber);
                }
            }
            return availableBarber;
        }

        //method that can be called from client console app
        [AllowAnonymous]
        public JsonResult GetAvailableBarber(string _date, string _startTime, string _endTime)
        {
            DateTime date = DateTime.ParseExact(_date, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime startTime = DateTime.ParseExact(_startTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(_endTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            //to avoid error 'circular reference was detected while serializing an object of type System.Data.Entity.DynamicProxies.MeetingRoom'
            //_context.Configuration.ProxyCreationEnabled = false;
            List<Barber> freeBarber = FindAvailableBarbers(date, startTime, endTime);

            return Json(freeBarber);//, JsonRequestBehavior.AllowGet);
        }

        //populate start time dropdown list
        private void PopulateStartTimeDropDownList(DateTime? selectedStartTime = null)
        {
            List<SelectListItem> startTimes = new List<SelectListItem>();
            DateTime date = DateTime.MinValue.AddHours(8); // start at 8am
            DateTime endDate = DateTime.MinValue.AddHours(18); // end at 6pm
            while (date < endDate)
            {
                //used for edits to pass on the time that was previously selected
                bool selected = false;
                if (selectedStartTime.HasValue)
                {
                    if (selectedStartTime.Value.Hour == date.Hour)
                    {
                        selected = true;
                    }
                }

                startTimes.Add(new SelectListItem { Text = date.ToShortTimeString(), Value = date.ToString(), Selected = selected });
                date = date.AddHours(1);
            }
            ViewBag.Start_DateTime = startTimes;
        }

        //populate end time dropdown list
        private void PopulateEndTimeDropDownList(DateTime? selectedEndTime = null)
        {
            List<SelectListItem> endTimes = new List<SelectListItem>();
            DateTime date = DateTime.MinValue.AddHours(9); // start at 8am
            DateTime endDate = DateTime.MinValue.AddHours(18); // end at 6pm
            while (date <= endDate)
            {
                //used for edits to pass on the time that was previously selected
                bool selected = false;
                if (selectedEndTime.HasValue)
                {
                    if (selectedEndTime.Value.Hour == date.Hour)
                    {
                        selected = true;
                    }

                }
                endTimes.Add(new SelectListItem { Text = date.ToShortTimeString(), Value = date.ToString(), Selected = selected });
                date = date.AddHours(1);
            }
            ViewBag.End_DateTime = endTimes;
        }

    }
}

