using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;

namespace SportsStore.UnitTests {

    [TestClass]
    public class AdminTests {

        [TestMethod]
        public void Index_Contains_All_Products() {

            // Arrange  - Create the Mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
            });

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0].Name, "P1");
            Assert.AreEqual(result[1].Name, "P2");
            Assert.AreEqual(result[2].Name, "P3");
        }

        [TestMethod]
        public void Can_edit_Product() {

            // Arrange  - Create the Mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
            });

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            // Assert - check if correct product was returned 
            Assert.IsNotNull(p1);
            Assert.IsNotNull(p2);
            Assert.IsNotNull(p3);

            Assert.AreEqual(p1.Name, "P1");
            Assert.AreEqual(p2.Name, "P2");
            Assert.AreEqual(p3.Name, "P3");
        }

        [TestMethod]
        public void Cannot_Edit_Noexistent_Product() {

            // Arrange  - Create the Mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
            });

            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product p5 = target.Edit(5).ViewData.Model as Product;

            // Assert
            Assert.IsNull(p5);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes() {

            // Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // arrange - create a new product
            Product product = new Product { Name = "Test" };

            // Act - try to save the product
            ActionResult result = target.Edit(product);

            // Assert - Check that the repository was called
            mock.Verify(m => m.SaveProduct(product));
            // Assert - check method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes() {

            // Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a new product
            Product product = new Product { Name = "Test" };
            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            ActionResult result = target.Edit(product);

            // Assert - Check that the repository was called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // Assert - check method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
