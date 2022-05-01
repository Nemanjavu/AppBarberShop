using AppBarberShop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace AppBarberShop.Models
{
    public class Customer : ApplicationUser
    {
        
        public int CustomerId { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }

    }
}
