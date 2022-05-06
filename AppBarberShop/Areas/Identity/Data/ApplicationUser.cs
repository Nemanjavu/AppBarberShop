﻿using System;
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
    //[Key]
    //public int Id { get; set; }
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; }

    //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //{
    // //Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //   // Add custom user claims here
    //    return userIdentity;
    //}
}

