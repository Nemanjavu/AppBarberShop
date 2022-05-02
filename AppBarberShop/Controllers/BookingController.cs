﻿

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
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            //List<Booking> bookings = _context.Bookings.ToList();
            //return View();
            IEnumerable<Booking> bookings;
            if (User.IsInRole("Admin"))
            {
                bookings = _context.Bookings.Where(b => b.Date >= DateTime.Today).Include(b => b.Barber).OrderBy(b => b.Date).ThenBy(b => b.Start_DateTime).ToList();
            }
            else
            {
                string user_id = User.Identity.Name;
                bookings = _context.Bookings.Where(b => b.UserId == user_id).Where(b => b.Date >= DateTime.Today).Include(b => b.Barber).OrderBy(b => b.Date).ThenBy(b => b.Start_DateTime).ToList();
            }

            return View(bookings);
        }

        // GET: Booking/Create
        //Used to Collect Criteria to Find Room
        public ActionResult Create()
        {
            PopulateStartTimeDropDownList();
            PopulateEndTimeDropDownList();
            return View();
        }

        // POST: Booking/CreateStep2
        //Used to Pick Room
        [HttpPost]
        public ActionResult CreateStep2(BookingCreateViewModel vm)
        {
            //if (vm == null)
            //{
            //    return new(BadRequest);
            //}
            if (vm.InvalidStartAndEnd())
            {
                ModelState.AddModelError("", "Please check start and end times. A meeting cannot end before it starts.");
            }
            if (ModelState.IsValid)
            {
                //find available rooms!
                List<Barber> availableBarbers = FindAvailableBarbers(vm.Date, vm.Start_DateTime, vm.End_DateTime);
                if (availableBarbers.Count == 0)
                {
                    ModelState.AddModelError("", "Sorry, there is no available room at the specified date and time.");
                }
                else
                {
                    Booking newBooking = new Booking
                    {
                        Date = vm.Date,
                        Start_DateTime = vm.Start_DateTime,
                        End_DateTime = vm.End_DateTime,
                    };
                    ViewBag.Username = User.Identity.Name;
                    ViewBag.AvailableRooms = availableBarbers;
                    ViewBag.RoomId = new SelectList(availableBarbers, "BarberId", "Name");
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
        public ActionResult CreatePost([Bind("BarberId, Date, Start_DateTime, End_DateTime, UserId")] Booking booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //make sure room is still free
                    foreach (Booking b in _context.Bookings)
                    {
                        if (!b.IsValidBooking(booking))
                        {
                            ModelState.AddModelError("", "This room is not available any longer for booking. Please try to make another booking.");
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
                if (!User.IsInRole("Admin") && booking.UserId != User.Identity.Name)
                {
                    throw new UnauthorizedAccessException("Oops, this booking doesn't seem to be yours, you cannot edit it.");
                }
                //using a viewmodel to pass on data from controller to view then to controller again when posting
                BookingEditViewModel vm = new BookingEditViewModel
                {
                    BookingId = booking.BookingId,
                    Date = booking.Date,
                    Start_DateTime = booking.Start_DateTime,
                    End_DateTime = booking.End_DateTime
                };

                ViewBag.RoomId = new SelectList(_context.Barbers, "BarberId", "BarberName", booking.BarberId);
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
            else
            {
                if (ModelState.IsValid)
                {
                    //create set of existing bookings
                    List<Booking> checkSet = _context.Bookings.ToList();
                    checkSet.Remove(bookingToUpdate);

                    //assign vm values to booking
                   
                    bookingToUpdate.Date = vm.Date;
                    bookingToUpdate.Start_DateTime = vm.Start_DateTime;
                    bookingToUpdate.End_DateTime = vm.End_DateTime;
                    bookingToUpdate.BarberId = vm.BarberId;

                    //make sure room is free                    
                    foreach (Booking b in checkSet)
                    {
                        if (!b.IsValidBooking(bookingToUpdate))
                        {
                            ModelState.AddModelError("", "The selected room is not available at the selected date and times. Please try to make another booking.");
                            ViewBag.RoomId = new SelectList(_context.Barbers, "RoomId", "Name", bookingToUpdate.BarberId);
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
            ViewBag.RoomId = new SelectList(_context.Barbers, "RoomId", "Name", bookingToUpdate.BarberId);
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
                if (!User.IsInRole("Admin") && booking.UserId != User.Identity.Name)
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

        //Find Available Rooms when date and time are picked
        private List<Barber> FindAvailableBarbers(DateTime date, DateTime startTime, DateTime endTime)
        {
            List<Barber> availableBarber = _context.Barbers.OrderBy(r => r.BarberName).ToList();
            //availableBarber = await _context.Barber.//OrderBy(r => r.BarberName).ToListAsync();

            //find meetings on same day
            IEnumerable<Booking> filteredBookings = _context.Bookings.Where(b => b.Date.Year == date.Year && b.Date.Month == date.Month && b.Date.Day == date.Day).ToList();

            //for each meeting at same time, eliminate room
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

