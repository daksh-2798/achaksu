using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projectnet.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace Projectnet.Controllers
{
    public class UserController : Controller
    {
        AchaksyuatDatabaseEntities1 m = new AchaksyuatDatabaseEntities1();
        // GET: User
        [HttpGet]
        public ActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]

        public ActionResult UserRegisterPost(string fname, string lname, string gender, int date, int month, int year, int bp, string udid, string certificate, string address, string city, string state, string country, string mob, string email, string password)
        {
            var data = new UserMaster
            {
                FName = fname,
                LName = lname,
                Gender = gender,
                Date = date,
                Month = month,
                Year = year,
                Blindprec = bp,
                UDID = udid,
                CertificateNo = certificate,
                Address = address,
                City = city,
                State = state,
                Country = country,
                Mobile = mob,
                Email = email,
                Password = password

            };
            m.UserMasters.Add(data);
            m.SaveChanges();
            return RedirectToAction("UserLogin");


        }
        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(FormCollection frm)
        {
            var myEmail = Convert.ToString(frm["email"]);
            var myPasswd = Convert.ToString(frm["password"]);
            TempData["myPassword"] = myPasswd;
            AchaksyuatDatabaseEntities1 chkLogin = new AchaksyuatDatabaseEntities1();
            var RdFound = chkLogin.UserMasters.Where(cust => cust.Email == myEmail && cust.Password == myPasswd).FirstOrDefault();
            if (RdFound != null)
            {
                //ViewData["ErrMsg"] = "Login Successfull";
                Session["Id"] = RdFound.Id;
                Session["Email"] = RdFound.Email;
                Response.Redirect("UserHome", false);
                //return View();
            }
            else
            {
                TempData["ErrMsg"] = "Invalid Email or Password";

            }
            return View();
        }
        public ActionResult UserHome()
        {
            //var RdFound = m.UserMasters.Where(cust => cust.Email == e).FirstOrDefault();

            //if (RdFound != null)
            //{
            //    //ViewData["ErrMsg"] = "Login Successfull";
            //    Session["Id"] = RdFound.Id;
            //    Session["Email"] = RdFound.Email;
            //    //Response.Redirect("./UserHome/", false);
            //    //return View();
            //    }
            return View();

        }

        [HttpGet]
        public ActionResult UserProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserProfilePost(string fname, string lname, string gender, int date, int month, int year, int bp, string udid, string certificate, string address, string city, string state, string country, string mob, string email, string password)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("update UserMaster set Address = '"+ address +"',City = '"+ city +"',State = '"+ state +"',Mobile= '"+ mob +"' where Email='"+ email +"' ");
            m.SaveChanges();
            return RedirectToAction("UserProfile");
        }
        public ActionResult UserDelivery()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ProdView(string e)
        {
            Session["val"] = 0;
            var RdFound = m.UserMasters.Where(cust => cust.Email == e).FirstOrDefault();

            if (RdFound != null)
            {
                //ViewData["ErrMsg"] = "Login Successfull";
                Session["Id"] = RdFound.Id;
                Session["Email"] = RdFound.Email;
                //Response.Redirect("./UserHome/", false);
                //return View();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Prodview(string e)
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i + 1;
            Session["val"] = i;
            return View();
        }
        [HttpPost]
        public ActionResult ProdViewBack()
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i - 1;
            Session["val"] = i;
            return View("Prodview");
        }
        public ActionResult AddtoCart(string pnp,string ppri, int quantity)
        {
            var data = new CartMaster
            {
                ProdNo = pnp,
                Price = Convert.ToInt32(ppri),
                Quantitty = quantity,
                total = (Convert.ToInt32(ppri) * quantity),
                UserEmail = Convert.ToString(Session["Email"])

        };

            m.CartMasters.Add(data);
            m.SaveChanges();
           
            return View("ProdView");
        }

        public ActionResult CartView()
        {
            return View();
        }

        public ActionResult UserOrder(string n, string daddr, string c,string s,string pc,string ct,string cemail,string paymethod,string o_status)
        {
            Class1 cls = new Class1();

            string ord = Convert.ToString(cls.auto("OrderId", "OrderMaster"));
            
            var data = new OrderMaster
            {
                OrderId =ord ,
                Customer_Name = n,
                Delivery_Address = daddr,
                City = c,
                State = s,
                Pincode = pc,
                Contact_No = ct,
                Customer_Email = cemail,
                Pay_Method = paymethod,
                Order_Status = o_status,
                Order_Date = DateTime.Now.Date,
                User_Email=Convert.ToString(Session["Email"])
                

            };

            m.OrderMasters.Add(data);
            m.SaveChanges();

            var sdata = new OrderStatu
            {
                OderId = ord,
                OrderStatus = o_status,
                Date = DateTime.Now.Date
            };
            m.OrderStatus.Add(sdata);
            m.SaveChanges();

            System.Data.DataSet ds1 = new System.Data.DataSet();
            ds1 = cls.selectDATA("select ProdNo,Price,Quantitty,total from CartMaster where UserEmail='" + Convert.ToString(Session["Email"]) + "' ");
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                var data1 = new OrderDetail
                {
                    Order_Id = ord,
                    Customer_Email = cemail,
                    User_Email = Convert.ToString(Session["Email"]),
                    Prod_No = Convert.ToString(ds1.Tables[0].Rows[i]["ProdNo"]),
                    Price = Convert.ToInt32(ds1.Tables[0].Rows[i]["Price"]),
                    Quantity = Convert.ToInt32(ds1.Tables[0].Rows[i]["Quantitty"]),
                    Total = Convert.ToInt32(ds1.Tables[0].Rows[i]["total"])


                };
                m.OrderDetails.Add(data1);
                m.SaveChanges();
            }

            cls.insert_up_del("delete from CartMaster where UserEmail = '" + Convert.ToString(Session["Email"]) + "' ");

            //Email Start

            var senderEmail = new MailAddress("achaksyuat@gmail.com", "Achaksyuat");
            var receiverEmail = new MailAddress(Convert.ToString(Session["Email"]), "Receiver");
            var password = "achaksyuat12345";
            var sub = "Order Confirmed";
            string bodytext = "Your Order has been placed successfully and will be processed in the earliest time.<br/>Details of your order..<br/>Order id :"+ord+" ";
            var body = bodytext;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body
            })
            {
                mess.IsBodyHtml = true;
                smtp.Send(mess);
            }
            //Email End
            return View();
        }

        public ActionResult Remove(int k)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("delete from CartMaster where ProdNo='"+Convert.ToString(k)+ "' and UserEmail = '" + Convert.ToString(Session["Email"]) + "' ");
            return RedirectToAction("CartView");
        }

        public ActionResult ClearCart()
        {
            Class1 cls = new Class1();
            cls.insert_up_del("delete from CartMaster where UserEmail = '" + Convert.ToString(Session["Email"]) + "' ");
            return RedirectToAction("CartView");
        }

        [HttpGet]
        public ActionResult YourOrderView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult YourOrderView(string e)
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i + 1;
            Session["val"] = i;
            return View();
        }

        public ActionResult OrderBackView()
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i - 1;
            Session["val"] = i;
            return View("YourOrderView");
        }

        public ActionResult Case()
        {
            return View();
        }

        public ActionResult UserOfferHome()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserOfferCat(string e)
        {
            Session["val"] = 0;
            var RdFound = m.UserMasters.Where(cust => cust.Email == e).FirstOrDefault();

            if (RdFound != null)
            {
                //ViewData["ErrMsg"] = "Login Successfull";
                Session["Id"] = RdFound.Id;
                Session["Email"] = RdFound.Email;
                //Response.Redirect("./UserHome/", false);
            }
            return View();
        }

        [HttpPost]
        public ActionResult UserOfferCatNext(string e)
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i + 1;
            Session["val"] = i;
            return View("UserOfferCat");
        }
        [HttpPost]
        public ActionResult UserOfferCatBack()
        {
            int i = Convert.ToInt32(Session["val"]);
            i = i - 1;
            Session["val"] = i;
            return View("UserOfferCat");
        }

        public ActionResult OfferAddtoCart(string pnp, string ppri, int quantity)
        {
            var data = new CartMaster
            {
                ProdNo = pnp,
                Price = Convert.ToInt32(ppri),
                Quantitty = quantity,
                total = (Convert.ToInt32(ppri) * quantity),
                UserEmail = Convert.ToString(Session["Email"])

            };

            m.CartMasters.Add(data);
            m.SaveChanges();

            return View("UserOfferCat");
        }
    }
}