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
    public class BarberControlerTests
    {
        private readonly ApplicationDbContext _context;
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Details_ShouldFail_WhenNullBarberID()
        {
            var controller = new BarberController(_context);
            
            var badResult = controller.Detail(null) as NotFoundResult;
        }
    }
}


