using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using Projectnet.Models;
using System.IO;
using System.Configuration;

namespace Projectnet.Controllers
{
    public class AdminController : Controller
    {
        AchaksyuatDatabaseEntities1 m = new AchaksyuatDatabaseEntities1();
        // GET: Admin

        [HttpGet]
        public ActionResult PackagerLogin()
        {
            if (Session["Name"] != null)
            {
                ViewData["Name"] = Convert.ToString(Session["Name"]);
            }
            return View();
        }
        [HttpPost]
        public ActionResult PackagerLogin(FormCollection frm)
        {
            var myEmail = Convert.ToString(frm["email"]);
            var myPasswd = Convert.ToString(frm["password"]);
            TempData["myPassword"] = myPasswd;
            AchaksyuatDatabaseEntities1 chkLogin = new AchaksyuatDatabaseEntities1();
            var AdFound = chkLogin.PackagerMasters.Where(p => p.Email == myEmail && p.Password == myPasswd).FirstOrDefault();
            if (AdFound != null)
            {
                ViewData["ErrMsg"] = "Login Successfull";
                //Session["EmailId"] = AdFound.Email;
                return RedirectToAction("PackagerView");

            }
            else
            {
                TempData["ErrMsg"] = "Invalid Email or Password";

            }
            return View();
        }
        [HttpGet]
        public ActionResult AdminRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminRegisterPost(string name, string gender, string date, string mob, string email, string password)
        {
            var adata = new AdminMaster
            {
                Name = name,
                Gender = gender,
                DateOB = date,
                Mobile = mob,
                Email = email,
                Password = password

            };
            m.AdminMasters.Add(adata);
            m.SaveChanges();
            return RedirectToAction("AdminLogin");
        }
        [HttpGet]
        public ActionResult AdminLogin()
        {
            if (Session["Name"] != null)
            {
                ViewData["Name"] = Convert.ToString(Session["Name"]);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(FormCollection frm)
        {
            var myEmail = Convert.ToString(frm["email"]);
            var myPasswd = Convert.ToString(frm["password"]);
            TempData["myPassword"] = myPasswd;
            AchaksyuatDatabaseEntities1 chkLogin = new AchaksyuatDatabaseEntities1();
            var AdFound = chkLogin.AdminMasters.Where(admin => admin.Email == myEmail && admin.Password == myPasswd).FirstOrDefault();
            if (AdFound != null)
            {
                ViewData["ErrMsg"] = "Login Successfull";
                //Session["BookId"] = RdFound.BookId;
                //Session["Name"] = RdFound.Name;
                return RedirectToAction("AdminHome");

            }
            else
            {
                TempData["ErrMsg"] = "Invalid Email or Password";

            }
            return View();
        }
        [HttpGet]
        public ActionResult AdminAddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminAddProductPost(string pno, string pname, string brand, string pdescrp, int price, int cid, HttpPostedFileBase postedfile, string imgcontent, int ds, int discount)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedfile.InputStream))
            {
                bytes = br.ReadBytes(postedfile.ContentLength);
            }

            string con1 = @"data source=.\sqlexpress;initial catalog=AchaksyuatDatabase;persist security info=True;user id=sa;password=Admin@123";
            //  string constr = ConfigurationManager.ConnectionStrings[con1].ConnectionString;
            using (SqlConnection con = new SqlConnection(con1))
            {
                string query = "INSERT INTO ProductMaster VALUES (@ProdNo, @ProdName, @Brand, @Description, @Price, @Discount, @CategoryId, @ProdImage, @ProdImgContent, @IsDiscount, @IsActive)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@ProdNo", pno);
                    cmd.Parameters.AddWithValue("@ProdName", pname);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@Description", pdescrp);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Discount", discount);
                    cmd.Parameters.AddWithValue("@CategoryId", cid);
                    cmd.Parameters.AddWithValue("@ProdImage", bytes);
                    cmd.Parameters.AddWithValue("@ProdImgContent", postedfile.ContentType);
                    cmd.Parameters.AddWithValue("@IsDiscount", ds);
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                }
            }


            return RedirectToAction("AdminHome");
        }
        [HttpGet]
        public ActionResult AdminAddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminAddCategory(string catid, string ctype)
        {
            var catdata = new CategoryMaster
            {
                Cat_Id = catid,
                Cat_Type = ctype

            };
            m.CategoryMasters.Add(catdata);
            m.SaveChanges();
            return View();
        }

        public ActionResult AdminHome()
        {
            return View();
        }

        public ActionResult AdminOrderView()
        {
            return View();
        }
        [HttpGet]
        public ActionResult PackagerRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PackagerRegisterPost(string name, string hub, string mob, string email, string password)
        {
            var adata = new PackagerMaster
            {
                Name = name,
                Hub_Name = hub,
                Mobile = mob,
                Email = email,
                Password = password

            };
            m.PackagerMasters.Add(adata);
            m.SaveChanges();
            return RedirectToAction("AdminHome");
        }

        public ActionResult OrderConfirm(int k)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("update OrderMaster set Order_Status='Confirmed' where OrderId=" + k + "  ");

            var sdata = new OrderStatu
            {
                OderId = Convert.ToString(k),
                OrderStatus = "Confirmed",
                Date = DateTime.Now.Date
            };
            m.OrderStatus.Add(sdata);
            m.SaveChanges();
            return RedirectToAction("AdminOrderView");
        }

        public ActionResult PackagerView()
        {
            var sLIst = m.OrderMasters.SqlQuery("select * from OrderMaster Where Order_Status='Confirmed'").ToList<OrderMaster>();


            return View(sLIst);
        }

        public ActionResult PackageDetails(string k)
        {
            return View();
        }
        public ActionResult PackageConfirm(string k)
        {
            Class1 c = new Class1();
            c.insert_up_del("update OrderMaster set Order_Status='Confirmed' where OrderId=" + k + "  ");
            return RedirectToAction("PackagerView");
        }

        public ActionResult OrderPackedDel(int k)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("update OrderMaster set Order_Status='Packed' where OrderId=" + k + " ");

            var sdata = new OrderStatu
            {
                OderId = Convert.ToString(k),
                OrderStatus = "Packed",
                Date = DateTime.Now.Date
            };
            m.OrderStatus.Add(sdata);
            m.SaveChanges();
            return RedirectToAction("PackagerView");
        }


        public ActionResult PackedOrder()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DeliveryCoRegister()
        {
            return View();
        }
            
        [HttpPost]
        public ActionResult DeliveryCoRegister(string name,string hub,string city,string state,string contact,string email,string password)
        {
            var data = new DeliveryCoMaster
            {
                CompName=name,
                HubName=hub,
                City=city,
                State=state,
                ContactNo=contact,
                EmailId=email,
                Password=password

            };
            m.DeliveryCoMasters.Add(data);
            m.SaveChanges();
            return RedirectToAction("AdminHome");
        }

        [HttpGet]
        public ActionResult DeliverLogin()
        {
            if (Session["Name"] != null)
            {
                ViewData["Name"] = Convert.ToString(Session["Name"]);
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeliverLogin(FormCollection frm)
        {
            var myEmail = Convert.ToString(frm["email"]);
            var myPasswd = Convert.ToString(frm["password"]);
            TempData["myPassword"] = myPasswd;
            AchaksyuatDatabaseEntities1 chkLogin = new AchaksyuatDatabaseEntities1();
            var AdFound = chkLogin.DeliveryCoMasters.Where(p => p.EmailId == myEmail && p.Password == myPasswd).FirstOrDefault();
            if (AdFound != null)
            {
                // ViewData["ErrMsg"] = "Login Successfull";
                Session["EmailId"] = AdFound.EmailId;
                return RedirectToAction("DeliveryCoView");

            }
            else
            {
                TempData["ErrMsg"] = "Invalid Email or Password";

            }

            return View();
        }

        [HttpGet]
        public ActionResult ShipProduct(int k,int p,int l)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShipProduct(int k,int p,string cname)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("update OrderMaster set Order_Status='Shipped' where OrderId=" + k + " ");

            var data = new ShipMAster
            {
                Order_Id = Convert.ToString(k),
                ProdNo=p,
                Del_Company=cname
            };
            m.ShipMAsters.Add(data);
            //m.SaveChanges();

            
            var sdata = new OrderStatu
            {
                OderId = Convert.ToString(k),
                OrderStatus = "Shipped",
                Date = DateTime.Now.Date
            };
            m.OrderStatus.Add(sdata);
            m.SaveChanges();
            return RedirectToAction("PackedOrder");
        }

        public ActionResult DeliveryCoView()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StockInView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StockInView(int p,int q,string r)
        {
            var data = new StockMaster
            {
                Prod_Id=p,
                Quantity=q,
                In_Date=DateTime.Now.Date,
                Remarks=r
            };
            m.StockMasters.Add(data);
            m.SaveChanges();
            return View();
        }

        public ActionResult StockView()
        {
            return View();
            
        }
        [HttpPost]
        public ActionResult AddDiscountCatView(string cid)
        {
            Response.Redirect("AddDiscountProdView?id=" + cid, true);
           // return RedirectToAction("AddDiscountProdView ? id = " + cid, true);
            return View();
        }
        [HttpGet]
        public ActionResult AddDiscountCatView()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult AddDiscountProdView(int cid)
        {
            
            var sList= m.ProductMasters.SqlQuery("SELECT * from ProductMaster where CategoryId ="+cid+" ").ToList<ProductMaster>();
            return View(sList);
            
        }

        [HttpPost]
        public ActionResult AddDiscountProdDisView(int pid)
        {
            var sList = m.ProductMasters.SqlQuery("SELECT * from ProductMaster where ProdNo =" + pid + " ").ToList<ProductMaster>();
            return View(sList);
        }

        public ActionResult UpdateDiscount(string  id,string d)
        {
            Class1 cls = new Class1();
            cls.insert_up_del("update ProductMaster set Discount="+  Convert.ToInt32( d) +" where ProdNo=" +  Convert.ToInt32( id) + " ");

            return RedirectToAction("AddDiscountCatView");
        }

        public ActionResult AdminPackageDetails(string k)
        {
            return View();
        }
    }
}
