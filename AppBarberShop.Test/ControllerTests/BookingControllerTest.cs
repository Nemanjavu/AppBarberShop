using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Controllers;
using AppBarberShop.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace AppBarberShop.Test.ControllerTests
{ 
    public class BookingControllerTests
    {
        private BookingController _bookingController;
        private ApplicationDbContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        public BookingControllerTests()
        {
            [Fact]
            public void Test_Index_Returns_Bookings()
            {
                // Arrange
                BookingController controller = new BookingController(_context);
                // Act
                ViewResult result = controller.Index() as ViewResult;
                // Assert
                Assert.IsNotNull(result);
            }
        }
        
    }
}
