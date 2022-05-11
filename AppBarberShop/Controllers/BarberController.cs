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
        public IActionResult Detail(int id)
        {
        
            Barber barbers = _context.Barbers.FirstOrDefault(b => b.BarberId == id);
            if (barbers == null)
            {
                return NotFound();
            }
            return View(barbers);
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