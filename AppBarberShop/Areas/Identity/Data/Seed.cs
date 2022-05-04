﻿using AppBarberShop.Models;

namespace AppBarberShop.Areas.Identity.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                if (!context.Barbers.Any())
                {
                    context.Barbers.AddRange(new List<Barber>()
                    {
                       new Barber()
                       {
                           BarberName = "John Smith",
                       },
                       new Barber()
                       {
                           BarberName = "Mary Smith"
                       }
                    });
                    context.SaveChanges();
                }

            }
        }
    }

}
