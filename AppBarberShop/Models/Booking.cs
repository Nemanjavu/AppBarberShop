

using AppBarberShop.Data.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBarberShop.Models
{
    
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Display(Name = "Choose service")]
        public Service Service { get; set; }

        [ForeignKey("BarberId")]
        [Display(Name = "Barber")]
        public int BarberId { get; set; }
        
        public  Barber Barber { get; set; }
        

        [Display(Name = "Date")]
        [Required(ErrorMessage = "please choose date")]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [ForeignKey("CustomerId")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        
        public virtual Customer Customer { get; set; }
        
        

        

        //public bool IsValidBooking(Booking newBooking)
        //{
        //    if (newBooking.BookingId == BarberId && newBooking.Date == Date)
        //    {
        //        return true;
        //        }
        //    else
        //     {
        //         return false;
        //        }
        //}
            
    }


}

