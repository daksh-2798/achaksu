using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Projectnet
{
    public class Class1
    {
   

    SqlConnection con = new SqlConnection(@"data source=.\sqlexpress;initial catalog=AchaksyuatDatabase;persist security info=True;user id=sa;password=Admin@123");


    public Boolean insert_up_del(string query)
    {
        SqlCommand cmd = new SqlCommand(query, con);
        try
        {
            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();
            return true;

        }
        catch
        {
            return false;
        }



    }


        public int auto(string filedname,string tablename)
        {


            SqlCommand cmd = new SqlCommand("select max(" + filedname + ") from " + tablename + "", con);
            try
            {
                con.Open();
                int a;
                a = Convert.ToInt32( cmd.ExecuteScalar());
                a = a + 1;
                con.Close();
                return a;

            }
            catch
            {
                return 1;
            }

        }
    public DataSet selectDATA(string query)
    {

        SqlDataAdapter adp = new SqlDataAdapter(query, con);
        DataSet ds = new DataSet();

        try
        {
            adp.Fill(ds);
            return ds;
        }
        catch
        {
            return null;
        }


    }

    }

}