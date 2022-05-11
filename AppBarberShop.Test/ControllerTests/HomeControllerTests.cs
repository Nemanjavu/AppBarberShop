using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Controllers;
using AppBarberShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.Mvc;


namespace AppBarberShop.Test
{
    

    [TestClass]
    public class HomeControllerTest
    {
        private readonly ILogger<HomeController> _logger;
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(_logger);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
