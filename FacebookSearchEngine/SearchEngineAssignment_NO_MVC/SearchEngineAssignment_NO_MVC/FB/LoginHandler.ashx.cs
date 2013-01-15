using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Database_Connector;
using MySql.Data.MySqlClient;

namespace SearchEngineAssignment_NO_MVC.FB
{
    /// <summary>
    /// Summary description for LoginHandler
    /// </summary>
    public class LoginHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        
        public void ProcessRequest(HttpContext context)
        {
            var accessToken = context.Request["accessToken"].Split('&')[0].Split('=')[1];
            string uid = context.Request["uid"];
            FacebookInteraction.access_token = accessToken;
            context.Session["token"] = accessToken;
            context.Session["uid"] = uid;
            Database database = new Database("b81ca2da-f0ca-4968-b9ef-a147009a4ef4.mysql.sequelizer.com",
                "dbb81ca2daf0ca4968b9efa147009a4ef4", "lizeabvjtqokfima", "qauWLTF4Db7umBPvvyy5LPYAzjLvtFMJKNKnbahQUaN7eEks6ndW4FvHi3vAhkH6");
            MySqlCommand command = database.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            string query = "SELECT COUNT(*) FROM facebook.users WHERE user_id = '" + uid + "'";
            database.Open();
            bool isNewUser;
            if (Convert.ToInt32(database.ExecuteScalar(query, command)) == 0)
            {
                query = "INSERT INTO facebook.users VALUES('" + uid + "', '" + accessToken + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                isNewUser = true;
            }
            else
            {
                query = "UPDATE facebook.users SET token = '" + accessToken + "' WHERE user_id = '" + uid + "'";
                isNewUser = false;
            }
            database.ExecuteNonQuery(query, command);

            database.Close();
            if (isNewUser)
            {
                context.Response.Redirect("../FirstTimeIndex.aspx");
            }
            else
            {
                context.Response.Redirect("../Search/Search.aspx");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}