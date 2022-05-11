//@Html.EditorFor(model => model.Service, new { htmlAttributes = new { @class = "form-control", @autofocus = "autofocus" } })

using AppBarberShop.Areas.Identity.Data;
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
        
        public virtual Barber Barber { get; set; }
        

        [Required(ErrorMessage = "Indicate booking date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Indicate when booking starts.")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime Start_DateTime { get; set; }

        [Required(ErrorMessage = "Indicate when booking ends.")]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime End_DateTime { get; set; }

        public string UserId { get; set; }
        
        public virtual ApplicationUser AppUser { get; set; }
        
        

        
        //Should Change the name
        public bool IsValidBooking(Booking newBooking)
        {//Changed newBooking.BookingId to Barberid
            if (newBooking.BarberId == BarberId && newBooking.Date == Date)
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

