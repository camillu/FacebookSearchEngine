using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SearchEngineAssignment_NO_MVC.FB;

namespace SearchEngineAssignment_NO_MVC
{
    public partial class FirstTimeIndex : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            Response.Flush();

            Indexer.Indexer indexer = new Indexer.Indexer();
            indexer.Index(FacebookInteraction.access_token);

            Response.Write("Done! You can now proceed to search.");
            Response.Flush();
        }
    }
}