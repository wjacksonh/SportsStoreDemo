using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities {
    public class Cart {
        private List<CartLine> lineCollection = new List<CartLine>();

        public IEnumerable<CartLine> Lines { get { return lineCollection; } }

        public void AddItem(Product product, int quantity) {
            CartLine line = lineCollection
                .Where(p => p.Product.ProductID == product.ProductID)
                .FirstOrDefault();

            if(line != null) {
                line.Quantity += quantity;
            } else {
                lineCollection.Add(new CartLine { Product = product, Quantity = quantity });
            }
        }

        public void RemoveLine(Product product) {
            lineCollection.RemoveAll(p => p.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTotalValue() {
            return lineCollection.Sum(l => l.Product.Price * l.Quantity);
        }

        public void Clear() {
            lineCollection.Clear();
        }
    }

    public class CartLine {
        public Product Product  { get; set; }
        public int     Quantity { get; set; }
    }
}
