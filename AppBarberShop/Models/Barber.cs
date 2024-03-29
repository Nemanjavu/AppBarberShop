﻿
using AppBarberShop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppBarberShop.Models
{
    public class Barber 
    {
        [Key]
        public int BarberId { get; set; }

        [Required(ErrorMessage = "Name cannot be left blank.")]
        [Display(Name = "Barber Name")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string  BarberName{ get; set; }
        [Required(ErrorMessage = "Description cannot be left blank.")]
        public string BarberDescription { get; set; }   
        public string BarberImage { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
