using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppBarberShop.Models;
using Microsoft.AspNetCore.Identity;

namespace AppBarberShop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
  
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; }

}

