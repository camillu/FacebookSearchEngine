using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SearchEngineAssignment_NO_MVC.FB;
using SearchEngineAssignment_NO_MVC.Indexer;
using Facebook;
using Database_Connector;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SearchEngineAssignment_NO_MVC.Search
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Redirect(object sender, EventArgs e)
        {
            Response.Redirect("Results.aspx?q=" + searchBox.Text);
        }
    }
}