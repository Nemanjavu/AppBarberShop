using AppBarberShop.Models;
using Microsoft.EntityFrameworkCore;

namespace AppBarberShop.Areas.Identity.Data
{
    public interface IBookingContext : IDisposable
    {
        DbSet<Barber> Barbers { get; }
        DbSet<Booking> Bookings { get; }

        int SaveChanges();
        void MarkAsModified(Object item);
    }
    
}
