using System.Linq;
using System.Web.Mvc;
using Northwind.Models;
using System.Data.Entity;
using System.Collections.Generic;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {


        public ActionResult ViewCart()
        {
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                List<CartData> cartData = new List<CartData>();


                var carts = db.Carts
                    .Include(p => p.Product)
                    .Include(c => c.Customer)
                    .ToList();

                foreach(Cart c in carts)
                {
                    CartData cd  = new CartData()
                    {
                        
                        ProductID = c.Product.ProductID,
                        CustomerID = c.Customer.CustomerID,
                        Quantity = c.Quantity,
                        ProductName = c.Product.ProductName,
                        UnitPrice = c.Product.UnitPrice,
                        UnitsInStock = c.Product.UnitsInStock
                    };
                    cartData.Add(cd);
                }

                return View(cartData);
            }


        }

        public JsonResult RefreshCart()
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                List<CartData> cartData = new List<CartData>();


                var carts = db.Carts
                    .Include(p => p.Product)
                    .Include(c => c.Customer)
                    .ToList();

                foreach (Cart c in carts)
                {
                    CartData cd = new CartData()
                    {

                        ProductID = c.Product.ProductID,
                        CustomerID = c.Customer.CustomerID,
                        Quantity = c.Quantity,
                        ProductName = c.Product.ProductName,
                        UnitPrice = c.Product.UnitPrice,
                        UnitsInStock = c.Product.UnitsInStock
                    };
                    cartData.Add(cd);
                }

                return Json(cartData, JsonRequestBehavior.AllowGet);
            }


        }

       


        //POST: Update Cart Quantity
        [HttpPost]
        public ActionResult UpdateCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            RemoveFromCart(cartDTO);



            AddToCart(cartDTO);
           // ViewCart();

            return new EmptyResult();
        }


        //POST: Remove From Cart
        [HttpPost]
        public JsonResult RemoveFromCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            

            using(NORTHWNDEntities db = new NORTHWNDEntities())
            {
                //AddToCart should ensure this works/ otherwise throw an exception
                Cart cart = db.Carts.Single(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID);
                db.Carts.Remove(cart);
                db.SaveChanges();
                
            }
           // ViewCart();
            return Json(sc, JsonRequestBehavior.AllowGet);
        }


        // POST: Cart/AddToCart
        [HttpPost]
        public JsonResult AddToCart(CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            // create cart item from Json object
            Cart sc = new Cart();
            sc.ProductID = cartDTO.ProductID;
            sc.CustomerID = cartDTO.CustomerID;
            sc.Quantity = cartDTO.Quantity;



            //using (NORTHWNDEntities db = new NORTHWNDEntities())
            //{
            //    // add the product to the customer’s cart
            //    db.Carts.Add(sc);
            //    db.SaveChanges();
            //}

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                // if there is a duplicate product id in cart, simply update the quantity
                if (db.Carts.Any(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID))
                {
                    // this product is already in the customer's cart,
                    // update the existing cart item's quantity
                    Cart cart = db.Carts.FirstOrDefault(c => c.ProductID == sc.ProductID && c.CustomerID == sc.CustomerID);
                    cart.Quantity += sc.Quantity;
                    sc = new Cart()
                    {
                        CartID = cart.CartID,
                        ProductID = cart.ProductID,
                        CustomerID = cart.CustomerID,
                        Quantity = cart.Quantity
                    };
                }
                else
                {
                    // this product is not in the customer's cart, add the product
                    db.Carts.Add(sc);
                }
                db.SaveChanges();
                System.Threading.Thread.Sleep(1500);
            }

            return Json(sc, JsonRequestBehavior.AllowGet);
        }
    }
}