using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace AppBarberShop.ViewModels
{
    public class BookingCreateViewModel
    {
        private DateTime start_DateTime;
        private DateTime end_DateTime;

        [HiddenInput]
        public int? BookingId { get; set; }

        [Required(ErrorMessage = "Indicate booking date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
      
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Indicate when booking starts.")]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public DateTime Start_DateTime
        {
            get
            {
                return start_DateTime;
            }
            set
            {
                start_DateTime = new DateTime(Date.Year, Date.Month, Date.Day, value.Hour, 0, 1);
            }
        }

        [Required(ErrorMessage = "Indicate when booking ends.")]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public DateTime End_DateTime
        {
            get
            {
                return end_DateTime;
            }
            set
            {
                end_DateTime = new DateTime(Date.Year, Date.Month, Date.Day, value.Hour, 0, 0);
            }
        }

        public bool InvalidStartAndEnd()
        {
            if (Start_DateTime >= End_DateTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool InvalidStartTime()
        {
            if (Start_DateTime <= DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool InValidDate()
        {
            if (Date < DateTime.Today)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    }
}
