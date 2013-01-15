using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using Database_Connector;
using MySql.Data.MySqlClient;
using System.Data;

namespace SearchEngineAssignment_NO_MVC.FB
{
    class FacebookInteraction
    {
        public static string access_token;
        private static Database database = new Database("b81ca2da-f0ca-4968-b9ef-a147009a4ef4.mysql.sequelizer.com",
                "dbb81ca2daf0ca4968b9efa147009a4ef4", "lizeabvjtqokfima", "qauWLTF4Db7umBPvvyy5LPYAzjLvtFMJKNKnbahQUaN7eEks6ndW4FvHi3vAhkH6");
        private static MySqlCommand command = database.CreateCommand();

        public static string GetUserID(string access_token)
        {
            FacebookClient client = new FacebookClient(access_token);
            dynamic me = client.Get("me");
            return me.id;
        }

        public static string GetUserName(string access_token)
        {
            FacebookClient client = new FacebookClient(access_token);
            dynamic me = client.Get("me");
            return me.name;
        }

        public static bool UpdateStatus(string access_token, string status)
        {
            try
            {
                FacebookClient client = new FacebookClient(access_token);
                Dictionary<string, object> postingArguments = new Dictionary<string, object>();
                postingArguments["message"] = status;
                client.Post("me/feed", postingArguments);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Logout(string access_token)
        {
            try
            {
                FacebookClient client = new FacebookClient(access_token);
                return client.GetLogoutUrl(new { access_token = access_token, next = "https://www.facebook.com/connect/login_success.html" }).ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Dictionary<string,string> GetFriends(string access_token)
        {
            FacebookClient client = new FacebookClient(access_token);
            dynamic list = client.Get("me/friends");
            int noOfFriends = (int)list.data.Count;
            Dictionary<string, string> id = new Dictionary<string, string>();
            for (int i = 0; i < noOfFriends; i++)
            {
                id.Add(list.data[i].id, list.data[i].name);
            }
            list = client.Get("me");
            id.Add(list.id, list.name);
            return id;
        }

        public static dynamic GetFeed(string access_token)
        {
            List<JToken> response = new List<JToken>();
            FacebookClient client = new FacebookClient(access_token);
            try
            {
                string nextpage = "me/feed?limit=200";

                while(true)
                {
                    var json = JObject.Parse(client.Get(nextpage).ToString());
                    var array = json["data"];
                    if (array == null)
                    {
                        break;
                    }
                    dynamic paging = json["paging"];
                    response.Add(array);
                    nextpage = paging.next.ToString();
                }
            }
            catch (RuntimeBinderException rbe)
            {
                Debug.WriteLine(rbe.TargetSite.Name + ": " + "Facebook API limit reached!");
            }
            return response;
        }

        public static KeyValuePair<string, string> GetProfileLink(string user_id)
        {
            string profile;
            string name;
            database.Open();
            command.CommandText = "SELECT * FROM friends WHERE friend_id = '" + user_id + "';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            database.Close();
            dt = set.Tables[0];
            DataRow result = dt.Rows[0];

            profile = "http://www.facebook.com/profile.php?id=" + user_id;
            name = result["name"].ToString();

            return new KeyValuePair<string, string>(profile, name);
        }
    }
}