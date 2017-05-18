using System.Linq;
using System.Web.Mvc;
using Northwind.Models;
using System.Data.Entity;
using System.Collections.Generic;

namespace Northwind.Controllers
{
    public class CartController : Controller
    {
       public void AddOrder(Order_Detail od)
        {
            using(NORTHWNDEntities db = new NORTHWNDEntities())
            {
                if (od.Product.Discontinued)
                {
                    var discount = db.Discounts.Single(d => d.ProductID == od.ProductID);
                    od.Discount = (decimal) discount.DiscountPercent;

                }

                db.Order_Details.Add(od);
                db.SaveChanges();
            }
            
        }

           

        public ActionResult CompleteOrder(int id)
        {
            
            
            List<Order_Detail> orderList = new List<Order_Detail>();
            Customer cust = null;

            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                 cust = db.Customers.Single(c => c.CustomerID == id);

                var cartList = db.Carts.Where(c => c.CustomerID == id);


                foreach (Cart c in cartList)
                {



                    Order_Detail od = new Order_Detail();
                    od.Product = c.Product;
                    od.Quantity = (short)c.Quantity;
                    od.UnitPrice = (decimal)c.Product.UnitPrice;
                    orderList.Add(od);


                    CartDTO cart = new CartDTO();
                    cart.CustomerID = (int)c.CustomerID;
                    cart.ProductID = (int)c.ProductID;
                    cart.Quantity = (int)c.Quantity;
                    this.RemoveFromCart(cart);


                   



                }

             

            }

            Session["orders"] = orderList;
            return View();
        }

        public ActionResult Test()
        {
            List<OrderDetailsDTO> orderDetails = new List<OrderDetailsDTO>();
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                Order_Detail od = new Order_Detail();


            }
                
            

            return new EmptyResult();
        }

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
               // System.Threading.Thread.Sleep(1500);
            }

            return Json(sc, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ThankYou(int id)
        {
            Customer cust = null;
            using (NORTHWNDEntities db = new NORTHWNDEntities())
            {
                cust = db.Customers.Single(c => c.CustomerID == id);
            }

            ViewBag.CompanyName = cust.CompanyName;
            ViewBag.CustName = cust.ContactName;
            ViewBag.address = cust.Address;
            ViewBag.city = cust.City;
            ViewBag.state = cust.Region;
            ViewBag.zip = cust.PostalCode;

            return View();
        }
       
       
    }
}