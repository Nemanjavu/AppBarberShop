

using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppBarberShop.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Booking> bookings = _context.Bookings.ToList();
            return View();
        }
    }
}
