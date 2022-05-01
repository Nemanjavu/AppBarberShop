using AppBarberShop.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace AppBarberShop.ViewModels
{
    public class BookingEditViewModel : BookingCreateViewModel
    {
        

        [Display(Name = "Choose service")]
        public Service Service { get; set; }

        [Required]
        [Display(Name = "Barber")]
        public int BarberId { get; set; }

        
    }
}
