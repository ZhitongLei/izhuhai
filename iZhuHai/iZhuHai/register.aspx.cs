using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iZhuHai.Model;

namespace iZhuHai
{
    public partial class register : System.Web.UI.Page
    {
        private User user = new User();

        protected void Page_Load(object sender, EventArgs e)
        {
           if (Page.IsPostBack)
            {
                String email = Request["email"];
                /*if(email!=null)
                    Response.Write(email);*/
                if (user.IsUserEntryExisted(email))
                {
                    ClientScriptManager cs = Page.ClientScript;
                    cs.RegisterStartupScript(this.GetType(), "RegisterScript",
                        "<script type=text/javascript> alert('该邮箱已经被使用!') </script>");
                }
                else
                {
                    String userName = Request.Form["names"];
                    String personFile = Request.Form["introduce"];

                    if (user.AddUser(email, userName, personFile))
                        Server.Transfer("~/registerSucceed.html");
                }
            }
        }
    }
}