using System.Web.Mvc;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private IProductsRepository repository;

        public AdminController(IProductsRepository repository) {
            this.repository = repository;
        }

        public ViewResult Index() {
            return View(repository.Products);
        }

        public ViewResult Edit(int productId) {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);

            return View(product);
        }

        public ActionResult Cancel(string productName) {

            TempData["cancelMessage"] = string.Format("Edit of {0} has been canceled", productName);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Product product) {
            if (ModelState.IsValid) {
                repository.SaveProduct(product);

                TempData["successMessage"] = string.Format("{0} has been saved", product.Name);

                return RedirectToAction("Index");
            } else {
                // there is somthing wrong with the data
                return View(product);
            }
        }
    }
}