using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Controllers;
using AppBarberShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;


namespace AppBarberShop.Test
{
    [TestClass]
    public class BookingControllerTests
    {
        private readonly ApplicationDbContext _context;
        private IHttpContextAccessor _httpContextAccessor;

        [TestMethod]
        public void Delete_ShouldFail_WhenNullID()
        {
            var controller = new BookingController(_context, _httpContextAccessor);
            var expected = (int)System.Net.HttpStatusCode.BadRequest;
            var badResult = controller.Delete(null) as BadRequestResult;

            Assert.AreEqual(expected, badResult.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Delete_ShouldFail_WhenIncorrectID()
        {
            var controller = new BookingController(_context, _httpContextAccessor);

            var badresult = controller.Delete(99999);
        }

        [TestMethod()]
        public void CreateTest_ShouldFailIfInvalidTimes()
        {
            BookingController controller = new BookingController(_context, _httpContextAccessor);
            BookingCreateViewModel bookingToAdd = new BookingCreateViewModel() { Date = new DateTime(2022, 05, 29), End_DateTime = new DateTime(2022, 05, 29, 10, 0, 0), Start_DateTime = new DateTime(2022, 05, 29,11, 0, 1) };

            var result = controller.Create2(bookingToAdd);

            Assert.IsTrue(!controller.ModelState.IsValid);
            Assert.IsTrue(controller.ViewData.ModelState.Count == 1,
                 "Please check start and end times. A meeting cannot end before it starts.");
        }
    }
}