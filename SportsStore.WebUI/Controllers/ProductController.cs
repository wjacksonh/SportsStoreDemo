using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        IProductsRepository repository;
        public int PageSize = 4;

        public ProductController (IProductsRepository repository) {
            this.repository = repository;
        }

        public ViewResult List (string category, int page = 1) {

            ProductsListViewModel model = new ProductsListViewModel {
                Products = repository.Products
                                     .Where(p => ((category == null) || (p.Category == category)))
                                     .OrderBy (p => p.ProductID)
                                     .Skip ((page - 1) * PageSize)
                                     .Take (PageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = (category == null) ? repository.Products.Count() :
                                                      repository.Products
                                                        .Where(p => p.Category == category).Count()
                },
                CurrentCategory = category
            };

            return View (model);
        }

        public FileContentResult GetImage(int productId) {
            Product product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);

            if(product != null) {
                return File(product.ImageData, product.ImageMimeType);
            } else {
                return null;
            }
        }
    }
}