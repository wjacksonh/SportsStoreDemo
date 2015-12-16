using System;
using System.Collections.Generic;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete {
    public sealed class EFProductRepository : IProductsRepository, IDisposable {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products {
            get { return context.Products; }
        }

        public Product DeleteProduct(int productId) {
            Product dbEntry = context.Products.Find(productId);

            if(dbEntry != null) {
                context.Products.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }

        public void SaveProduct(Product product) {
            
            if(product.ProductID == 0) {
                context.Products.Add(product);
            } else {
                Product dbEntry = context.Products.Find(product.ProductID);

                if(dbEntry != null) {
                    dbEntry.Name = product.Name;
                    dbEntry.Price = product.Price;
                    dbEntry.Description = product.Description;
                    dbEntry.Category = product.Category;
                    dbEntry.ImageData = product.ImageData;
                    dbEntry.ImageMimeType = product.ImageMimeType;
                }
            }
            context.SaveChanges();
        }

        public void Dispose() {
            if (context != null)
                context.Dispose();
        }
    }
}