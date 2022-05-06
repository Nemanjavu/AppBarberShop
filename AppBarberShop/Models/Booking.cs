

using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Data.Enum;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace AppBarberShop.Models
{
    
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

       // [Display(Name = "Choose service")]
        public Service Service { get; set; }

        [ForeignKey("BarberId")]
        [Display(Name = "Barber")]
        public int BarberId { get; set; }
      
        public virtual Barber Barber { get; set; }
        

        [Required(ErrorMessage = "Indicate meeting date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Indicate when meeting starts.")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime Start_DateTime { get; set; }

        [Required(ErrorMessage = "Indicate when meeting ends.")]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime End_DateTime { get; set; }

        //[Required]
        //[Display(Name = "Customer")]
       // [ForeignKey("UserId")]
        public string UserId { get; set; }
        //[ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        //public virtual Customer Customer { get; set; }

        



        public bool IsValidBooking(Booking newBooking)
        {
            if (newBooking.BookingId == BarberId && newBooking.Date == Date)
            {
                if ((newBooking.Start_DateTime > End_DateTime) || (newBooking.End_DateTime < Start_DateTime))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

    }


}

