using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using System.Web.Mvc;

namespace SportsStore.UnitTests {

    [TestClass]
    public class UnitTest1 {

        private Mock<IProductsRepository> CreateMockProductRepository () {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup (m => m.Products).Returns (new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1" },
                new Product {ProductID = 2, Name = "P2", Category = "Cat2" },
                new Product {ProductID = 3, Name = "P3", Category = "Cat1" },
                new Product {ProductID = 4, Name = "P4", Category = "Cat2" },
                new Product {ProductID = 5, Name = "P5", Category = "Cat3" },
            });

            return mock;
        }

        private Mock<IProductsRepository> CreateMockProductRepository2() {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
                new Product {ProductID = 2, Name = "P2", Category = "Apples" },
                new Product {ProductID = 3, Name = "P3", Category = "Plums" },
                new Product {ProductID = 4, Name = "P4", Category = "Oranges" },
            });

            return mock;
        }

        [TestMethod]
        public void Can_Paginate () {
            //Arrange
            Mock<IProductsRepository> mock = CreateMockProductRepository ();

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue (prodArray.Length == 2);
            Assert.AreEqual (prodArray[0].Name, "P4");
            Assert.AreEqual (prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model () {

            // Arrange
            Mock<IProductsRepository> mock = CreateMockProductRepository();

            // Arrange
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual (pageInfo.CurrentPage, 2);
            Assert.AreEqual (pageInfo.ItemsPerPage, 3);
            Assert.AreEqual (pageInfo.TotalItems, 5);
            Assert.AreEqual (pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Generate_Page_Links () {
            // Arrange - define HTML helper - we need to do this 
            // inorder to apply the extension method.
            HtmlHelper myHelper = null;

            // Arrange - create PageInfo data
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage  = 2,
                ItemsPerPage = 10,
                TotalItems   = 28
            };

            // Arrange - setup the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = (i => "Page" + i);

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual (@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                           + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                           + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                           result.ToString());
        }

        [TestMethod]
        public void Can_Filter_products () {
            // Arrange
            // Create a mock repository
            Mock<IProductsRepository> mock = CreateMockProductRepository ();

            // Arrange 
            // Mage a product controller object and make page size 3
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model)
                .Products.ToArray();

            // Assert
            Assert.AreEqual (result.Length, 2);
            Assert.IsTrue (result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue (result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories() {
            // Arrange
            // Create the mock repository
            Mock<IProductsRepository> mock = CreateMockProductRepository2();

            // Arrange - create the controller
            NavController controller = new NavController(mock.Object);

            // Act - get the set of categories
            MenuInfo result = (MenuInfo)controller.Menu().Model;
            string[] results = result.Categories.ToArray();

            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category() {

            // Arrange - create the mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Apples" },
                new Product {ProductID = 2, Name = "P2", Category = "Oranges" }
            });

            // Arrange - create the controller
            NavController controller = new NavController(mock.Object);

            // Arange - define the category to be selected
            string categoryToSelect = "Apples";

            // Action 
            MenuInfo result = (MenuInfo)controller.Menu(categoryToSelect).Model;
            string selectedCategory = result.SelectedCategory;

            // Assert
            Assert.AreEqual(selectedCategory, categoryToSelect);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count() {

            // Arrange - create mock repository
            Mock<IProductsRepository> mock = CreateMockProductRepository ();

            // Arrange - create product controller
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List("Cat1").Model;
            int totalItems1 = result.PagingInfo.TotalItems;
            result = (ProductsListViewModel)controller.List("Cat2").Model;
            int totalItems2 = result.PagingInfo.TotalItems;
            result = (ProductsListViewModel)controller.List("Cat3").Model;
            int totalItems3 = result.PagingInfo.TotalItems;
            result = (ProductsListViewModel)controller.List(null).Model;
            int totalItemsAll = result.PagingInfo.TotalItems;


            // Assert
            Assert.AreEqual(totalItems1, 2);
            Assert.AreEqual(totalItems2, 2);
            Assert.AreEqual(totalItems3, 1);
            Assert.AreEqual(totalItemsAll, 5);
        }
    }
}
