using AppBarberShop.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppBarberShop.Areas.Identity.Data
{
    public class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                
                if(!context.Barbers.Any())
                {
                    context.Barbers.AddRange(new List<Barber>()
                    {
                       new Barber()
                       {
                           BarberName = "John Smith",
                           BarberDescription = "10 years experience; you won't get a better fade.",
                           BarberImage = "https://as2.ftcdn.net/v2/jpg/02/70/15/15/1000_F_270151517_2TqUuCXRguak4jT6uYEekvKNn15A5oCx.jpg", 
                           
                       },
                       new Barber()
                       {
                           BarberName = "Mary Dolan",
                           BarberDescription = "5 years experience with my training done in the French School of cuts.",
                           BarberImage = "https://as2.ftcdn.net/v2/jpg/03/10/72/21/1000_F_310722163_fvLciF2seWfeybpfTvpwjNfBRnHQMsrr.jpg"

                       },
                       new Barber()
                       {
                           BarberName = "Jackie Byrne",
                           BarberDescription = "Life experience, for all your bearded or clean shaven styels. ",
                           BarberImage = "https://as1.ftcdn.net/v2/jpg/00/27/46/98/1000_F_27469876_kR6FhSxLHIh6CiNT0hMlG0kPQENnQV81.jpg"
                       }
                    });
                    context.SaveChanges();
                }
                
            }
        }
    }
}
