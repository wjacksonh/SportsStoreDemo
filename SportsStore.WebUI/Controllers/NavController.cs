using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductsRepository repository;

        public NavController(IProductsRepository respository) {
            this.repository = respository;
        }

        // GET: Nav
        public PartialViewResult Menu (string category = null) {

            IEnumerable<string> categories = repository.Products
                                                       .Select(x => x.Category)
                                                       .Distinct()
                                                       .OrderBy(x => x);

            MenuInfo menuInfo = new MenuInfo { Categories = categories, SelectedCategory = category };

            return PartialView("FlexMenu", menuInfo);
        }
    }
}