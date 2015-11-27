using System.Web.Mvc;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Abstract;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductsRepository repository;
        private IAuthProvider authProvider;

        public AdminController(IProductsRepository repository, IAuthProvider authProvider) {
            this.repository = repository;
            this.authProvider = authProvider;
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

        public ViewResult Create() {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete (int productId) {
            Product deletedProduct = repository.DeleteProduct(productId);

            if(deletedProduct != null) {
                TempData["successMessage"] = string.Format("{0} has been deleted", deletedProduct.Name);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Logout() {
            authProvider.SignOut();

            return Redirect(Url.Action("List", "Product"));
        }
    }
}