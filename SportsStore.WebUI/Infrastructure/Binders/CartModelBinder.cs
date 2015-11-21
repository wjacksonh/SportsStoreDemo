using System.Web.Mvc;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure.Binders {
    public class CartModelBinder : IModelBinder {

        private const string sessionKey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            // get the cart from the session
            Cart cart = null;

            if (controllerContext.HttpContext.Session  != null) {
                cart = (Cart)controllerContext.HttpContext.Session[sessionKey];
            }

            // Create the cart if it does not exist
            if(cart == null) {
                cart = new Cart();
                if (controllerContext.HttpContext.Session != null) {
                    controllerContext.HttpContext.Session[sessionKey] = cart;
                }
            }

            return cart;
        }
    }
}