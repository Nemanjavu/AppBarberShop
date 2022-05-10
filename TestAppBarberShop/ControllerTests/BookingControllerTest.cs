using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBarberShop.Areas.Identity.Data;
using AppBarberShop.Controllers;
using AppBarberShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppBarberShop.Test.ControllerTests
{
    [TestClass]
    public class BookingControllerTests
    {
        [TestMethod]
            public void Index()
            {
                // Arrange
                BookingController controller = new BookingController();

                // Act
                ViewResult result = controller.Index() as ViewResult;

                // Assert
                Assert.IsNotNull(result);
            }
        }
    }
}
