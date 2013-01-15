using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Database_Connector;
using MySql.Data.MySqlClient;
using SearchEngineAssignment_NO_MVC.Indexer;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using SearchEngineAssignment_NO_MVC.FB;

namespace SearchEngineAssignment_NO_MVC.Search
{
    public partial class Results : System.Web.UI.Page
    {
        Database database;
        MySqlCommand command;
        Dictionary<string, DictionaryStruct> Dictionary = new Dictionary<string, DictionaryStruct>();
        Dictionary<string, DoubleDictionaryStruct> TermFrequency = new Dictionary<string, DoubleDictionaryStruct>();
        List<KeyValuePair<int, WordStructure>> Postings = new List<KeyValuePair<int, WordStructure>>();
        Dictionary<string, StoryStruct> StoryDictionary = new Dictionary<string, StoryStruct>();
        List<KeyValuePair<string, string>> Likes = new List<KeyValuePair<string, string>>();
        Dictionary<string, string> Friends = new Dictionary<string, string>();
        HashSet<string> TopFriends = new HashSet<string>();
        Dictionary<string, CommentStruct> CommentDictionary = new Dictionary<string, CommentStruct>();
        HashSet<string> tokens;
        string query;
        Dictionary<string, string> acronyms = new Dictionary<string, string>();
        List<KeyValuePair<string, DoubleDictionaryStruct>> tf;
        Dictionary<int, double> idf;
        string user_id;
        protected void Page_Load(object sender, EventArgs e)
        {
            user_id = Session["uid"].ToString();
            acronyms.Add("2", "To");
            acronyms.Add("24/7", "Twenty-four hours a day, seven days a week");
            acronyms.Add("411", "Information");
            acronyms.Add("AFAIK", "As far as I know");
            acronyms.Add("AFK", "Away from keyboard");
            acronyms.Add("AIM", "AOL Instant Messenger");
            acronyms.Add("AKA", "Also known as");
            acronyms.Add("AM", "Antemeridian");
            acronyms.Add("AOL", "America Online");
            acronyms.Add("ASAP", "As soon as possible");
            acronyms.Add("ASL", "Age, sex, location");
            acronyms.Add("b/c", "Because");
            acronyms.Add("B/W", "Between");
            acronyms.Add("b4", "Before");
            acronyms.Add("BBIAB", "Be back in a bit");
            acronyms.Add("BBL", "Be back later");
            acronyms.Add("BBS", "be back soon");
            acronyms.Add("BCC", "Blind carbon copy");
            acronyms.Add("bf", "Boyfriend");
            acronyms.Add("BFF", "Best friends forever");
            acronyms.Add("BFN", "Bye for now");
            acronyms.Add("BOT", "Back on topic");
            acronyms.Add("BRB", "Be right back");
            acronyms.Add("BTW", "By the way");
            acronyms.Add("CC", "Carbon copy");
            acronyms.Add("CTN", "Can't talk now");
            acronyms.Add("cya", "See ya");
            acronyms.Add("CYE", "Check your e-mail");
            acronyms.Add("DIY", "Do it yourself");
            acronyms.Add("DL", "Download");
            acronyms.Add("ETA", "Estimated time of arrival");
            acronyms.Add("f", "Female");
            acronyms.Add("FAQ", "Frequently Asked Questions");
            acronyms.Add("fb", "Facebook");
            acronyms.Add("FUBAR", "Fouled up beyond all recognition");
            acronyms.Add("FWIW", "For what it's worth");
            acronyms.Add("FYI", "For your information");
            acronyms.Add("gb", "Goodbye");
            acronyms.Add("gf", "Girlfriend");
            acronyms.Add("GG", "Good game");
            acronyms.Add("GJ", "Good job");
            acronyms.Add("GL", "Good luck");
            acronyms.Add("gr8", "Great");
            acronyms.Add("GTG", "Got to go");
            acronyms.Add("HOAS", "Hold on a second");
            acronyms.Add("HTH", "Hope this helps");
            acronyms.Add("hw", "Homework");
            acronyms.Add("IAC", "In any case");
            acronyms.Add("IC", "I see");
            acronyms.Add("IDK", "I don't know");
            acronyms.Add("IIRC", "If I remember correctly");
            acronyms.Add("IM", "Instant Message");
            acronyms.Add("IMO", "In my opinion");
            acronyms.Add("IRT", "In regards to");
            acronyms.Add("J/K", "Just kidding");
            acronyms.Add("K", "OK");
            acronyms.Add("L8", "Late");
            acronyms.Add("L8R", "Later");
            acronyms.Add("LMAO", "Laughing my a** off");
            acronyms.Add("LMK", "Let me know");
            acronyms.Add("LOL", "Laughing out loud");
            acronyms.Add("m", "Male");
            acronyms.Add("MMB", "Message me back");
            acronyms.Add("MMO", "Massively Multiplayer Online");
            acronyms.Add("msg", "Message");
            acronyms.Add("MYOB", "Mind your own business");
            acronyms.Add("N/A", "Not Available");
            acronyms.Add("NC", "No comment");
            acronyms.Add("ne1", "Anyone");
            acronyms.Add("NM", "Not much");
            acronyms.Add("NP", "No problem");
            acronyms.Add("NTN", "No thanks needed");
            acronyms.Add("OMG", "Oh my gosh");
            acronyms.Add("OT", "Off topic");
            acronyms.Add("PHAT", "Pretty hot and tempting");
            acronyms.Add("PK", "Player Kill");
            acronyms.Add("pls", "Please");
            acronyms.Add("PM", "Postmeridian");
            acronyms.Add("POS", "Parent over shoulder");
            acronyms.Add("ppl", "People");
            acronyms.Add("pwn", "Own");
            acronyms.Add("qt", "Cutie");
            acronyms.Add("re", "Regarding");
            acronyms.Add("ROFL", "Rolling on the floor laughing");
            acronyms.Add("ROTFL", "Rolling on the floor laughing");
            acronyms.Add("RPG", "Role Playing Game");
            acronyms.Add("RSVP", "R�pondez s'il vous pla�t");
            acronyms.Add("RTFM", "Read the flippin' manual");
            acronyms.Add("SOS", "Someone over shoulder");
            acronyms.Add("Sry", "Sorry");
            acronyms.Add("sup", "What's up");
            acronyms.Add("TBA", "To be announced");
            acronyms.Add("TBC", "To be continued");
            acronyms.Add("TBD", "To be determined");
            acronyms.Add("TC", "Take care");
            acronyms.Add("thx", "Thanks");
            acronyms.Add("TIA", "Thanks in advance");
            acronyms.Add("TLC", "Tender love and care");
            acronyms.Add("TMI", "Too much information");
            acronyms.Add("TTFN", "Ta-ta for now");
            acronyms.Add("TTYL", "Talk to you later");
            acronyms.Add("Tweet", "Twitter Post");
            acronyms.Add("txt", "Text");
            acronyms.Add("TY", "Thank you");
            acronyms.Add("u", "You");
            acronyms.Add("U2", "You too");
            acronyms.Add("UR", "Your");
            acronyms.Add("VM", "Voicemail");
            acronyms.Add("W/", "With");
            acronyms.Add("w/e", "Whatever");
            acronyms.Add("w/o", "Without");
            acronyms.Add("W8", "Wait");
            acronyms.Add("WB", "Write back");
            acronyms.Add("XOXO", "Hugs and kisses");
            acronyms.Add("Y", "Why");
            acronyms.Add("YW", "You're welcome");
            acronyms.Add("ZZZ", "Sleeping");


            query = Request.QueryString["q"];
            string originalquery = query;

            
            HashSet<string> seen = new HashSet<string>();
            Dictionary<string, string> stemmedacronyms = new Dictionary<string, string>();
            Dictionary<string, string> final = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in acronyms)
            {
                string acronym = kvp.Key;
                string[] tempArr = kvp.Value.Split(' ');
                foreach (string s in tempArr)
                {
                    acronym = acronym.Replace(s, new EnglishStemmer.EnglishWord(s).Stem);
                }

                stemmedacronyms.Add(acronym, kvp.Value);
            }

            DateTime begin = DateTime.Now;
            
            foreach (KeyValuePair<string, string> kvp in stemmedacronyms)
            {

                if ((query.ToLower().Contains(" " + kvp.Key.ToLower() + " ") | query.ToLower().StartsWith(kvp.Key.ToLower() + " ") | query.ToLower().EndsWith(" " + kvp.Key.ToLower()) | query.ToLower() == kvp.Key.ToLower()) && !seen.Contains(kvp.Value))
                {

                    query += " " + kvp.Value;
                    seen.Add(kvp.Key);
                    continue;
                }

                if ((query.ToLower().Contains(" " + kvp.Value.ToLower() + " ") | query.ToLower().StartsWith(kvp.Value.ToLower() + " ") | query.ToLower().EndsWith(" " + kvp.Value.ToLower()) | query.ToLower() == kvp.Value.ToLower()) && !seen.Contains(kvp.Value))
                {
                    query += " " + kvp.Key;
                    seen.Add(kvp.Key);
                    continue;
                }
            }

            tokens = Tokenize(query);

            List<KeyValuePair<string, double>> results = Search(query);
            DateTime end = DateTime.Now;
            if (results == null)
            {
                SearchResultsPlaceHolder.Controls.Add(new Literal() { Text = "Invalid search query entered. Please try again." });
                Response.Flush();
                return;
            }

            if (results.Count == 0)
            {
                SearchResultsPlaceHolder.Controls.Add(new Literal() { Text = TextToHtml.ToParagraph("Sorry, no results for your query!") });
            }
            else
            {
                int resultcounter = 0;
                SearchResultsPlaceHolder.Controls.Add(new Panel() { CssClass = "spacer1" });
                Panel titlePanel = new Panel();
                titlePanel.CssClass = "titletext";
                Literal resultCount = new Literal();
                titlePanel.Controls.Add(resultCount);
                SearchResultsPlaceHolder.Controls.Add(new Panel() { CssClass = "mglass" });
                SearchResultsPlaceHolder.Controls.Add(titlePanel);
                SearchResultsPlaceHolder.Controls.Add(new Panel() { CssClass = "spacer2" });
                List<string> done = new List<string>();
                int stories = 0;
                List<KeyValuePair<string, double>> relevance = new List<KeyValuePair<string, double>>();
                List<KeyValuePair<string, string>> currentLikes = new List<KeyValuePair<string, string>>();
                foreach (KeyValuePair<string, double> kvp in results)
                {
                    string doc_id = kvp.Key;
                    double relevancecounter = Likes.FindAll(x => x.Key == doc_id && TopFriends.Contains(x.Value)).Count * 0.1;

                    relevance.Add(new KeyValuePair<string,double>(kvp.Key, relevancecounter + kvp.Value + StoryDictionary[doc_id].likes / 40));
                    currentLikes.Clear();
                }

                results = relevance;
                List<KeyValuePair<string,double>> sorted = results.OrderByDescending(r => r.Value).ToList();
      
                foreach (KeyValuePair<string, double> kvp in sorted)
                {
                    string doc_id = kvp.Key;
                    currentLikes.Clear();
                    if (done.Contains(doc_id))
                    {
                        continue;
                    }
                    else
                    {
                        done.Add(doc_id);
                    }
                    resultcounter++;
                    if (!StoryDictionary.ContainsKey(doc_id) || !Friends.ContainsKey(StoryDictionary[doc_id].owner))
                    {
                        continue;
                    }
                    KeyValuePair<string, string> owner = new KeyValuePair<string, string>(StoryDictionary[doc_id].owner, Friends[StoryDictionary[doc_id].owner]);
                    StoryPanel story = new StoryPanel("story");
                    StoryPanel storyheader = new StoryPanel("storyheader");
                    HyperLink link = storyheader.AddLink("lfloat", owner.Value, "http://www.facebook.com/" + owner.Key);
                    link.Target = "_parent";
                    storyheader.AddDiv("center");
                    storyheader.AddLink("ownername", owner.Value, "http://www.facebook.com/" + owner.Key);
                    link.Controls.Add(new Image() { CssClass = "ownerpic", ImageUrl = "http://graph.facebook.com/" + owner.Key + "/picture" });
                    story.AddDiv(storyheader);
                    StoryPanel storytext = new StoryPanel("storytext");
                    string text = StoryDictionary[doc_id].story;
                    foreach (string token in tokens)
                    {
                        text = Regex.Replace(text, token, "<span class=\"bold\">" + token + "</span>", RegexOptions.IgnoreCase);
                    }

                    storytext.AddParagraph(text);
                    story.AddDiv(storytext);


                    currentLikes = Likes.FindAll(x => x.Key == doc_id);
                    
                    if (currentLikes.Count > 0)
                    {
                        StoryPanel likebar = new StoryPanel("likebar");
                        Panel thumbsup = new Panel();
                        thumbsup.CssClass = "thumbsup";
                        likebar.Controls.Add(thumbsup);

                        if (currentLikes.Count == 1)
                        {
                            if (Friends.ContainsKey(currentLikes[0].Value))
                            {
                                likebar.AddLink("liker", Friends[currentLikes[0].Value], "http://www.facebook.com/" + currentLikes[0].Value);
                                likebar.AddLiteral(" likes this.");
                            }
                        }
                        else
                        {
                            likebar.AddLink("liker", currentLikes.Count + " people", "");
                            likebar.AddLiteral(" like this.");
                        }
                        likebar.Controls.Add(new Panel() { CssClass = "clear" });
                        story.AddDiv(likebar);
                    }

                    SearchResultsPlaceHolder.Controls.Add(story);
                    stories++;

                    int occurrences = 0;
                    foreach (string token in tokens)
                    {
                        if (!Dictionary.ContainsKey(token))
                        {
                            continue;
                        }
                        occurrences += Postings.FindAll(p => p.Key == Dictionary[token].word_id && p.Value.Document == doc_id).Count;
                    }
                    if (occurrences != 0)
                    {
                        Dictionary<string, CommentStruct> comments = new Dictionary<string, CommentStruct>();
                        foreach (KeyValuePair<string, CommentStruct> kvp2 in CommentDictionary)
                        {
                            if (kvp2.Value.storyID == doc_id)
                            {
                                comments.Add(kvp2.Key, kvp2.Value);
                            }
                        }
                        foreach (KeyValuePair<string, CommentStruct> kvp2 in comments)
                        {
                            if (Friends.ContainsKey(kvp2.Value.owner))
                            {
                                owner = new KeyValuePair<string, string>(kvp2.Value.owner, Friends[kvp2.Value.owner]);
                            }
                            else
                            {
                                owner = new KeyValuePair<string, string>(kvp2.Value.owner, "Not friends with this person");
                            }
                            StoryPanel comment = new StoryPanel("comment");
                            StoryPanel commentheader = new StoryPanel("storyheader");
                            HyperLink commentlink = commentheader.AddLink("lfloat", owner.Value, "http://www.facebook.com/" + owner.Key);
                            commentlink.Target = "_parent";
                            commentheader.AddDiv("center");
                            if (!owner.Value.StartsWith("N"))
                            {
                                commentheader.AddLink("ownername", owner.Value, "http://www.facebook.com/" + owner.Key);
                            }
                            else
                            {
                                commentheader.AddLink("notfriends", owner.Value, "http://www.facebook.com/" + owner.Key);
                            }
                            commentlink.Controls.Add(new Image() { CssClass = "ownerpic", ImageUrl = "http://graph.facebook.com/" + owner.Key + "/picture" });
                            comment.AddDiv(commentheader);
                            StoryPanel commenttext = new StoryPanel("storytext");
                            string text2 = kvp2.Value.comment;
                            foreach (string token in tokens)
                            {
                                text2 = Regex.Replace(text2, token, "<span class=\"bold\">" + token + "</span>", RegexOptions.IgnoreCase);
                            }
                            commenttext.AddParagraph(text2);
                            comment.AddDiv(commenttext);
                            SearchResultsPlaceHolder.Controls.Add(comment);
                        }
                    }
                }

                resultCount.Text = "Search for query \"" + originalquery + "\" returned " + stories + " results. (Page generated in: " + decimal.Round((decimal)(((TimeSpan)(end - begin)).TotalMilliseconds / 1000.0), 3, MidpointRounding.AwayFromZero) + " seconds.)";

                tokens.Clear();
                tokens = null;
            }
        }

        protected List<KeyValuePair<string, double>> Search(string toSearch)
        {
            string searchQuery = toSearch;
            query = searchQuery;

            if (searchQuery == null || searchQuery == "")
            {
                return null;
            }

            database = new Database("b81ca2da-f0ca-4968-b9ef-a147009a4ef4.mysql.sequelizer.com",
                "dbb81ca2daf0ca4968b9efa147009a4ef4", "lizeabvjtqokfima", "qauWLTF4Db7umBPvvyy5LPYAzjLvtFMJKNKnbahQUaN7eEks6ndW4FvHi3vAhkH6");
            command = database.CreateCommand();
            command.CommandTimeout = 0;

            tokens = Tokenize(searchQuery);

            database.Open();

            LoadFriends();
            LoadTopFriends();
            LoadLikes();
            LoadDictionary();
            LoadPostings();
            LoadDocuments();
            LoadComments();

            tf = GetTermFrequency();
            idf = GetDocumentFrequency();

            Dictionary<string, Dictionary<int, double>> tfidf = GetTFIDF();

            

            database.Close();

            List<KeyValuePair<string, double>> results = new List<KeyValuePair<string, double>>();

            foreach(KeyValuePair<string, Dictionary<int,double>> kvp in tfidf)
            {
                foreach(double relevance in kvp.Value.Values)
                {
                    if (relevance > 0)
                    {
                        results.Add(new KeyValuePair<string, double>(kvp.Key, relevance));
                    }
                }
            }
            return results;

        }

        private void LoadTopFriends()
        {
            command.CommandText = "SELECT friend_id FROM facebook.likes WHERE uid = '" + user_id + "' GROUP BY friend_id ORDER BY COUNT(friend_id) DESC LIMIT 5";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                TopFriends.Add(current[0].ToString());
            }
        }

        private void LoadFriends()
        {
            command.CommandText = "SELECT * FROM facebook.friends WHERE uid = '" + user_id + "';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                Friends.Add((string)current[0], (string)current[1]);
            }
        }

        private void LoadLikes()
        {
            command.CommandText = "SELECT * FROM facebook.likes WHERE uid = '" + user_id + "';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                Likes.Add(new KeyValuePair<string, string>((string)current[1], (string)current[2]));
            }
        }

        private void LoadDictionary()
        {
            command.CommandText = "SELECT * FROM facebook.dictionary WHERE uid = '" + user_id + "';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                Dictionary.Add((string)current[0], new DictionaryStruct() { word_id = (int)current[1], frequency = (int)current[2] });
            }
        }

        private List<KeyValuePair<string,DoubleDictionaryStruct>> GetTermFrequency()
        {
            List<KeyValuePair<string, DoubleDictionaryStruct>> tf = new List<KeyValuePair<string, DoubleDictionaryStruct>>();

            foreach (KeyValuePair<int, WordStructure> kvp in Postings)
            {
                tf.Add(new KeyValuePair<string, DoubleDictionaryStruct>(kvp.Value.Document, new DoubleDictionaryStruct() { word_id = kvp.Value.Id, termfrequency = (kvp.Value.TermFrequency / ComputeEuclideanNorm(kvp.Value.Document)) }));
            }

            return tf;
        }

        private double ComputeEuclideanNorm(string document_id)
        {
            double euclideanNorm = 0.0;

            foreach (KeyValuePair<int, WordStructure> kvp in Postings)
            {
                if (kvp.Value.Document == document_id)
                {
                    euclideanNorm += Math.Pow(kvp.Value.TermFrequency, 2);
                }
            }
            return Math.Sqrt(euclideanNorm);
        }

        private void LoadDocuments()
        {
            command.CommandText = "SELECT * FROM facebook.documents WHERE parent_story IS NULL AND uid = '" + user_id + "';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                StoryDictionary.Add((string)current[1], new StoryStruct() { story = (string)current[2], owner = (string)current[4], likes = Convert.ToInt32(current[6]) });
            }
        }

        private void LoadPostings()
        {
            string tokenstring = string.Empty;
            string onlytoken = null;
            foreach (string token in tokens)
            {
                if (!Dictionary.ContainsKey(token))
                {
                    onlytoken = token;
                    continue;
                }
                tokenstring += Dictionary[token].word_id + ",";
            }
            if (tokenstring == "")
            {
                return;
            }
            else
            {
                tokenstring = tokenstring.Substring(0, tokenstring.Length - 1);
            }
            command.CommandText = "SELECT * FROM facebook.postings WHERE word_id IN(" + tokenstring + ") AND uid = '" + user_id + "' ORDER BY document_id ASC";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                Postings.Add(new KeyValuePair<int, WordStructure>((int)current[1], new WordStructure((string)current[2], (int)current[1], (int)current[3])));
            }
        }

        private void LoadComments()
        {
            command.CommandText = "SELECT * FROM facebook.documents WHERE parent_story IS NOT NULL AND uid = '" + user_id +"';";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            foreach (DataRow current in dt.Rows)
            {
                CommentDictionary.Add((string)current[1], new CommentStruct() { comment = (string)current[2], storyID = (string)current[3], owner = (string)current[4] });
            }
        }

        private Dictionary<int, double> GetDocumentFrequency()
        {
            Dictionary<int, double> idf = new Dictionary<int, double>();

            int documentCount = Convert.ToInt32(database.ExecuteScalar("SELECT COUNT(*) FROM facebook.documents WHERE uid = '" + user_id + "';", command));

            foreach (KeyValuePair<string, DictionaryStruct> kvp in Dictionary)
            {
                idf.Add(Dictionary[kvp.Key].word_id, Math.Log(documentCount / kvp.Value.frequency, 2.0));
            }

            return idf;
        }

        private Dictionary<string, Dictionary<int, double>> GetTFIDF()
        {
            int termcount = Dictionary.Count;
            Dictionary<string, Dictionary<int, double>> tfidf = new Dictionary<string, Dictionary<int, double>>();
            foreach (string document_id in StoryDictionary.Keys)
            {
                Dictionary<int, double> inner = new Dictionary<int, double>();
                foreach (KeyValuePair<string, DoubleDictionaryStruct> kvp in tf)
                {
                    if (kvp.Key == document_id)
                    {
                        if (!inner.ContainsKey(kvp.Value.word_id))
                        {
                            inner.Add(kvp.Value.word_id, kvp.Value.termfrequency * idf[kvp.Value.word_id] / termcount);
                        }
                    }
                }
                tfidf.Add(document_id, inner);
            }
            return tfidf;
        }

        private HashSet<string> Tokenize(string searchQuery)
        {

            List<string> words = searchQuery.Split(' ').ToList<string>();
            for (int i = 0; i < words.Count; i++)
            {
                words[i] = Regex.Replace(words[i], @"[^\w\s]", " ");
                words[i] = Regex.Replace(words[i], @"\s+", " ");
                words[i] = Regex.Replace(words[i], @"\d", "");
                words[i] = words[i].ToLower();
            }
            words.RemoveAll(item => item == null);

            HashSet<string> temp = new HashSet<string>();

            foreach (string word in words)
            {
                string[] tempwords = word.Split(' ');
                foreach (string t in tempwords)
                {
                    if (!temp.Contains(t))
                    {
                        temp.Add(new EnglishStemmer.EnglishWord(t).Stem);
                    }
                }
            }

            temp.Remove(string.Empty);

            return temp;
        }
    }
}