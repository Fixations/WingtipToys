using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext _cartdb = new ProductContext();

        public const string CartSessionKey = "CartId";

        public void AddToCart(int id)
        {
            // Retrieve the product from the Database.
            ShoppingCartId = GetCartId();

            var cartItem = _cartdb.ShoppingCartItems.SingleOrDefault(
                c => c.CartId == ShoppingCartId && c.ProductId == id);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists.
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = _cartdb.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _cartdb.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart.
                // Just add one to quantity.
                cartItem.Quantity++;
            }

            _cartdb.SaveChanges();
        }

        public void Dispose()
        {
            if (_cartdb != null)
            {
                _cartdb.Dispose();
                _cartdb = null;
            }
        }

        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                // See if we have a customer Name and use it. If not create a temp GUID.
                if (!String.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.GUID class
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            return _cartdb.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId).ToList();
        }

        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();
            // Multiply product price by quantity of that product in the cart to get
            // the current price for each of the products in the cart.
            // Sum all products price totals to get the Grand cart total.
            decimal? total = decimal.Zero;
            total = (decimal?)(from cartItems in _cartdb.ShoppingCartItems
                               where cartItems.CartId == ShoppingCartId
                               select (int?)cartItems.Quantity *
                               cartItems.Product.UnitPrice).Sum();
            return total ?? decimal.Zero;
        }

        public ShoppingCartActions GetCart(HttpContext context)
        {
            using (var cart = new ShoppingCartActions())
            {
                cart.ShoppingCartId = cart.GetCartId();
                return cart;
            }
        }

        public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] CartItemUpdates)
        {
            using (var db = new WingtipToys.Models.ProductContext())
            {
                try
                {
                    int CartItemCount = CartItemUpdates.Count();
                    List<CartItem> myCart = GetCartItems();
                        foreach (var cartItem in myCart)
                        {
                            // Go through all rows within Shopping Cart List
                            for (int i = 0; i < CartItemCount; i++)
                            {
                                if (cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
                                {
                                    if (CartItemUpdates[i].PurchaseQuantity < 1 || CartItemUpdates[i].RemoveItem == true)
                                    {
                                        RemoveItem(cartId, cartItem.ProductId);
                                    }
                                    else
                                    {
                                        UpdateItem(cartId, cartItem.ProductId, CartItemUpdates[i].PurchaseQuantity);
                                    }
                                }
                            }
                        }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
                }
            }
        }

        public void RemoveItem(string removeCartID, int removeProductID)
        {
            using (var _cartdb = new WingtipToys.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in _cartdb.ShoppingCartItems
                                  where c.CartId == removeCartID && c.Product.ProductID == removeProductID
                                  select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        // Remove Item from cart
                        _cartdb.ShoppingCartItems.Remove(myItem);
                        _cartdb.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
                }
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            using (var _cartdb = new WingtipToys.Models.ProductContext())
            {
                try
                {
                    var myItem = (from c in _cartdb.ShoppingCartItems
                                  where c.CartId == updateCartID && c.Product.ProductID == updateProductID
                                  select c).FirstOrDefault();
                    if (myItem != null)
                    {
                        // If myItem is already in cart, add to total number
                        myItem.Quantity = quantity;
                        _cartdb.SaveChanges();
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();
            var cartItems = _cartdb.ShoppingCartItems.Where(
                c => c.CartId == ShoppingCartId);
            // Remove Items
            foreach (var cartItem in cartItems)
            {
                _cartdb.ShoppingCartItems.Remove(cartItem);
            }

            // Save changes
            _cartdb.SaveChanges();
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();

            // get the count of each item in the cart and sum them up.
            int? count = (from cartItems in _cartdb.ShoppingCartItems
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Quantity).Sum();
            // Return 0 if all enteries are null.
            return count ?? 0;
        }

        public struct ShoppingCartUpdates
        {
            // Struct assembly to tie shopping cart variables together.
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}