using System;
using System.Linq;
using Moq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests {

    [TestClass]
    public class CartTests {

        [TestMethod]
        public void Can_Add_New_Lines() {

            // Arrange - create some test Products
            Product p1 = new Product {ProductID = 1, Name = "P1" };
            Product p2 = new Product {ProductID = 2, Name = "P2" };

            // Arrange
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            // Assert
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Line() {

            // Arrange - create some test Products
            Product p1 = new Product {ProductID = 1, Name = "P1" };
            Product p2 = new Product {ProductID = 2, Name = "P2" };

            // Arrange
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.ToArray();

            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line() {

            // Arrange - create some test Products
            Product p1 = new Product {ProductID = 1, Name = "P1" };
            Product p2 = new Product {ProductID = 2, Name = "P2" };
            Product p3 = new Product {ProductID = 3, Name = "P3" };

            // Arrange
            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            // Act
            target.RemoveLine(p2);

            // Assert
            Assert.AreEqual(target.Lines.Where(p => p.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Can_Calculate_Cart_Total() {

            // Arrange - create some test Products
            Product p1 = new Product {ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product {ProductID = 2, Name = "P2", Price = 50M};
  
            // Arrange
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            decimal total = target.ComputeTotalValue();

            // Assert
            Assert.AreEqual(total, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents() {

            // Arrange - create some test Products
            Product p1 = new Product {ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product {ProductID = 2, Name = "P2", Price = 50M};

            // Arrange
            Cart target = new Cart();

            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            // Assert
            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart() {

            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" }
            }.AsQueryable());

            // Arange - create a cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController controller = new CartController(mock.Object, null);

            // Act - add a product to the cart
            controller.AddToCart(cart, 1, null);

            // Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_cart_Goes_To_Cart_Screen() {

            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" }
            }.AsQueryable());

            // Arange - create a cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController controller = new CartController(mock.Object, null);

            // Act - add a product to the cart
            RedirectToRouteResult result = controller.AddToCart(cart, 2, "myUrl");

            // Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents() {

            // Arange - create a cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController target = new CartController(null, null);

            // Act - add a product to the cart
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            // Assert
            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [TestMethod]
        public void Can_Remove_All_From_Cart() {

            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
                new Product {ProductID = 2, Name = "P2", Category = "Oranges" }
            }.AsQueryable());

            // Arange - create a cart
            Cart cart = new Cart();

            // Arrange - create the controller
            CartController controller = new CartController(mock.Object, null);
            controller.AddToCart(cart, 1, null);
            controller.AddToCart(cart, 2, null);

            // Act - add a product to the cart
            controller.RemoveAll(cart, null);

            // Assert
            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart() {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Arrange - create an empty cart
            Cart cart = new Cart();

            // Arrange - create shipping details
            ShippingDetails shippingDetails = new ShippingDetails();

            // Arrange - create cart controller
            CartController controller = new CartController(null, mock.Object);

            // Act
            ViewResult result = controller.Checkout(cart, shippingDetails);

            // Assert - check that the order has not been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_Shipping_Details() {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create cart controller
            CartController controller = new CartController(null, mock.Object);
            // Arrange - add an error to the model
            controller.ModelState.AddModelError("error", "error");

            // Act
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            // Assert - check that the order has not been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order() {
            // Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create cart controller
            CartController controller = new CartController(null, mock.Object);

            // Act
            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            // Assert - check that the order has not been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            // Assert - check that the method is returning the default view
            Assert.AreEqual("Completed", result.ViewName);
            // Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
