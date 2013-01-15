using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Database_Connector;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System.Data;
using SearchEngineAssignment_NO_MVC.FB;

namespace SearchEngineAssignment_NO_MVC.Indexer
{
    class Indexer
    {
        public Dictionary<string, DictionaryStruct> ReferenceDictionary = new Dictionary<string, DictionaryStruct>();
        public Dictionary<string, DictionaryStruct> Dictionary = new Dictionary<string, DictionaryStruct>();
        public List<KeyValuePair<int, WordStructure>> postings = new List<KeyValuePair<int, WordStructure>>();
        public Dictionary<string, StoryStruct> referenceStoryDictionary = new Dictionary<string, StoryStruct>();
        public Dictionary<string, StoryStruct> storyDictionary = new Dictionary<string, StoryStruct>();
        public Dictionary<string, CommentStruct> referenceCommentDictionary = new Dictionary<string, CommentStruct>();
        public List<KeyValuePair<string,string>> likes = new List<KeyValuePair<string,string>>();
        private Database database;
        MySqlCommand command;
        private int word_id = 1;
        string user_id;
        string[] stopwords = { "I", "a", "about", "an", "are", "as", "at", "be", "by", "com", "for", "from", "how", "in", "is", "it", "of", "on", 
                                "or", "that", "the", "this", "to", "was", "what", "when", "where", "who", "will", "with", "the", "www" };
        public Indexer()
        {
            database = new Database("b81ca2da-f0ca-4968-b9ef-a147009a4ef4.mysql.sequelizer.com",
                "dbb81ca2daf0ca4968b9efa147009a4ef4", "lizeabvjtqokfima", "qauWLTF4Db7umBPvvyy5LPYAzjLvtFMJKNKnbahQUaN7eEks6ndW4FvHi3vAhkH6");
            command = database.CreateCommand();
            command.CommandTimeout = 0;
        }

        public void Index(string accesstoken)
        {
            user_id = FacebookInteraction.GetUserID(accesstoken);
            Dictionary<string,string> friends = FacebookInteraction.GetFriends(accesstoken);
            database.Open();
            word_id = Convert.ToInt32(database.ExecuteScalar("SELECT id FROM facebook.dictionary ORDER BY id DESC LIMIT 1", command)) + 1;
            string query = "DELETE FROM friends WHERE uid = '" + user_id + "'";
            database.ExecuteNonQuery(query, command);
            foreach (KeyValuePair<string,string> kvp in friends)
            {
                    query = "INSERT INTO friends VALUES('" + kvp.Key + "', '" + kvp.Value.Replace("\'","\'\'") + "', '" + user_id + "')";
                    database.ExecuteNonQuery(query, command);
            }

            LoadDictionary("dictionary");
            LoadDictionary("documents");

            var stories = FacebookInteraction.GetFeed(accesstoken);

            foreach (JToken t in stories)
            {
                foreach (object o in t)
                {
                    try
                    {
                        if (((dynamic)o).message != null)
                        {
                            if (!referenceStoryDictionary.ContainsKey((string)((dynamic)o).id))
                            {
                                try
                                {
                                    Parse((string)((dynamic)o).message, (string)((dynamic)o).id, (string)((dynamic)o).from.id, (string)((dynamic)o).likes.count);
                                }
                                catch (RuntimeBinderException)
                                {
                                    Parse((string)((dynamic)o).message, (string)((dynamic)o).id, (string)((dynamic)o).from.id, "0");
                                }
                                try
                                {
                                    foreach (dynamic like in ((dynamic)o).likes.data)
                                    {
                                        likes.Add(new KeyValuePair<string, string>((string)((dynamic)o).id, like.id.ToString()));
                                    }
                                }
                                catch (RuntimeBinderException) { }
                            }
                            ParseComments(((dynamic)o).comments, (string)((dynamic)o).id);
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        Debug.WriteLine("Error: Story is of an invalid type or has no likes and/or comments.");
                    }
                }
            }

            Commit();
            database.Close();
            ReferenceDictionary.Clear();
            Dictionary.Clear();
            postings.Clear();
            referenceStoryDictionary.Clear();
            storyDictionary.Clear();
            referenceCommentDictionary.Clear();
            likes.Clear();
            return;
        }

        public void Parse(string toParse, string story_id, string owner, string likes)
        {
            Dictionary<string, int> documentfrequency = new Dictionary<string, int>();
            string refined = toParse.Replace("\'", "");
            refined = Regex.Replace(refined, @"[^\w\s]", " ");
            refined = Regex.Replace(refined, @"\s+", " ");
            refined = Regex.Replace(refined, @"\d", "");

            while (refined != "" && refined[0] == ' ')
            {
                refined = refined.Substring(1);
            }
            if (refined != null && !referenceStoryDictionary.ContainsKey(story_id) && !storyDictionary.ContainsKey(story_id))
            {
                referenceStoryDictionary.Add(story_id, new StoryStruct() { story = toParse.Replace("\'","\'\'"), owner = owner, likes = Convert.ToInt32(likes) });
                storyDictionary.Add(story_id, new StoryStruct() { story = toParse.Replace("\'", "\'\'"), owner = owner, likes = Convert.ToInt32(likes) });
            }
            string currentWord = null;
            bool seen = false;
            int i = 0;
            while (i < refined.Length)
            {
                if (refined.Contains(' '))
                {
                    while (refined[i] != ' ')
                    {
                        currentWord += refined[i];
                        i++;
                        if (i >= refined.Length)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    currentWord = refined;
                    i += refined.Length;
                }

                string expansion;
                if (refined != null && currentWord != null)
                {
                    expansion = currentWord.ToLower();
                    expansion = new EnglishStemmer.EnglishWord(expansion).Stem;
                }
                else
                {
                    continue;
                }

                if (expansion == null)
                {
                    continue;
                }

                if (stopwords.Contains<string>(expansion))
                {
                    i += expansion.Length;
                    continue;
                }

                if(documentfrequency.ContainsKey(expansion))
                {
                    seen = true;
                }

                if (!ReferenceDictionary.ContainsKey(expansion))
                {
                    ReferenceDictionary.Add(expansion, new DictionaryStruct() { word_id = word_id, frequency = 1 });
                    Dictionary.Add(expansion, new DictionaryStruct() { word_id = word_id, frequency = 1 });
                    word_id++;
                }
                else if(!seen)
                {
                    DictionaryStruct temp = ReferenceDictionary[expansion];
                    if (!Dictionary.ContainsKey(expansion))
                    {
                        Dictionary.Add(expansion, new DictionaryStruct() { frequency = 1, word_id = word_id });
                        word_id++;
                    }
                    else
                    {
                        Dictionary[expansion] = new DictionaryStruct() { frequency = temp.frequency + 1, word_id = Dictionary[expansion].word_id };
                    }
                }
                if (!documentfrequency.ContainsKey(expansion))
                {
                    documentfrequency.Add(expansion, 1);
                }
                else
                {
                    documentfrequency[expansion]++;
                }

                i++;
                currentWord = "";
            }

            foreach (KeyValuePair<string, int> kvp in documentfrequency)
            {
                postings.Add(new KeyValuePair<int, WordStructure>(ReferenceDictionary[kvp.Key].word_id, new WordStructure(story_id, ReferenceDictionary[kvp.Key].word_id, kvp.Value)));
            }
        }


        public void ParseComments(dynamic commentsStruct, string story_id)
        {

            if (commentsStruct.data == null)
            {
                return;
            }

            foreach (dynamic comment in commentsStruct.data)
            {
                Dictionary<string, int> documentfrequency = new Dictionary<string, int>();
                bool seen = false;
                string toParse = comment.message;
                string refined = toParse.Replace("\'", "");
                refined = Regex.Replace(refined, @"[^\w\s]", " ");
                refined = Regex.Replace(refined, @"\s+", " ");
                refined = Regex.Replace(refined, @"\d", "");

                while (refined != "" && refined[0] == ' ')
                {
                    refined = refined.Substring(1);
                }
                if (refined != null && !referenceStoryDictionary.ContainsKey(comment.id.ToString()) && !referenceCommentDictionary.ContainsKey(comment.id.ToString()))
                {
                    referenceCommentDictionary.Add(comment.id.ToString(), new CommentStruct() { comment = toParse.Replace("\'", "\'\'").Replace("\\", "\\\\").Replace(",", "\\,"), storyID = story_id, owner = comment.from.id });
                }

                if (referenceStoryDictionary.ContainsKey(comment.id.ToString()))
                {
                    continue;
                }

                string currentWord = null;
                int i = 0;
                while (i < refined.Length)
                {
                    if (refined.Contains(' '))
                    {
                        while (refined[i] != ' ')
                        {
                            currentWord += refined[i];
                            i++;
                            if (i >= refined.Length)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        currentWord = refined;
                        i += refined.Length;
                    }

                    string expansion;
                    if (refined != null && currentWord != null)
                    {
                        expansion = currentWord.ToLower();
                        expansion = new EnglishStemmer.EnglishWord(expansion).Stem;
                    }
                    else
                    {
                        continue;
                    }

                    if (expansion == null)
                    {
                        continue;
                    }

                    if (documentfrequency.ContainsKey(expansion))
                    {
                        seen = true;
                    }

                    if (!ReferenceDictionary.ContainsKey(expansion))
                    {
                        ReferenceDictionary.Add(expansion, new DictionaryStruct() { word_id = word_id, frequency = 1 });
                        Dictionary.Add(expansion, new DictionaryStruct() { word_id = word_id, frequency = 1 });
                        word_id++;
                    }
                    else if (!seen)
                    {
                        DictionaryStruct temp = ReferenceDictionary[expansion];
                        if (!Dictionary.ContainsKey(expansion))
                        {
                            Dictionary.Add(expansion, new DictionaryStruct() { frequency = 1, word_id = word_id });
                            word_id++;
                        }
                        else
                        {
                            Dictionary[expansion] = new DictionaryStruct() { frequency = temp.frequency + 1, word_id = Dictionary[expansion].word_id };
                        }
                    }
                    if (!documentfrequency.ContainsKey(expansion))
                    {
                        documentfrequency.Add(expansion, 1);
                    }
                    else
                    {
                        documentfrequency[expansion]++;
                    }

                    i++;
                    currentWord = "";
                }

                foreach (KeyValuePair<string, int> kvp in documentfrequency)
                {
                    postings.Add(new KeyValuePair<int, WordStructure>(ReferenceDictionary[kvp.Key].word_id, new WordStructure(story_id, ReferenceDictionary[kvp.Key].word_id, kvp.Value)));
                }
            }
        }

        private void Commit()
        {
            string query = string.Empty;

            foreach (KeyValuePair<string, DictionaryStruct> kvp in Dictionary)
            {
                command.CommandText = "SELECT docfreq FROM dictionary WHERE word = '" + kvp.Key + "' AND uid = '" + user_id + "'";
                MySqlDataAdapter adap = new MySqlDataAdapter(command);
                DataSet set = new DataSet();
                adap.Fill(set);
                DataTable dt = set.Tables[0];
                int freq = 0;
                if (dt.Rows.Count != 0)
                {
                    freq = (int)dt.Rows[0][0];
                }
                
                if (freq == 0)
                {
                    query = "INSERT INTO dictionary VALUES('" + kvp.Key + "', " + kvp.Value.word_id + ", " + kvp.Value.frequency + ", '" + user_id + "')";
                }
                else
                {
                    query = "UPDATE dictionary SET docfreq = " + (freq + kvp.Value.frequency) + " WHERE word = '" + kvp.Key + "' AND uid = '" + user_id + "'";
                }
                database.ExecuteNonQuery(query, command);
            }

            foreach (KeyValuePair<int, WordStructure> kvp in postings)
            {
                query = "INSERT INTO postings VALUES(default, " + kvp.Key + ", '" + kvp.Value.Document + "', " + kvp.Value.TermFrequency + ", '" + user_id + "')";
                database.ExecuteNonQuery(query, command);
            }

            foreach (KeyValuePair<string, StoryStruct> kvp in storyDictionary)
            {
                query = "INSERT INTO documents VALUES(default, '" + kvp.Key + "', '" + kvp.Value.story + "', NULL, '" + kvp.Value.owner + "', '" + user_id + "', " + kvp.Value.likes + ")";
                database.ExecuteNonQuery(query, command);
            }

            foreach (KeyValuePair<string, CommentStruct> kvp in referenceCommentDictionary)
            {
                query = "INSERT INTO documents VALUES(default, '" + kvp.Key + "', '" + kvp.Value.comment + "', '" + kvp.Value.storyID + "', '" + kvp.Value.owner + "', '" + user_id + "', NULL)";
                database.ExecuteNonQuery(query, command);
            }

            foreach (KeyValuePair<string, string> kvp in likes)
            {
                query = "INSERT INTO likes VALUES(default, '" + kvp.Key + "', '" + kvp.Value + "', '" + user_id + "')";
                database.ExecuteNonQuery(query, command);
            }
        }

        private void LoadDictionary(string dictionary)
        {
            command.CommandText = "SELECT * FROM facebook." + dictionary + " WHERE uid = '" + user_id + "'";
            MySqlDataAdapter adap = new MySqlDataAdapter(command);
            DataSet set = new DataSet();
            DataTable dt;
            adap.Fill(set);
            dt = set.Tables[0];

            switch (dictionary)
            {
                case "dictionary":
                    foreach (DataRow current in dt.Rows)
                    {
                        ReferenceDictionary.Add(current[0].ToString(), new DictionaryStruct() { word_id = (int)current[1], frequency = (int)current[2] });
                    }
                    break;
                case "documents":
                    foreach (DataRow current in dt.Rows)
                    {
                        referenceStoryDictionary.Add(current[1].ToString(), new StoryStruct() { story = current[2].ToString(), owner = current[4].ToString(), likes = DBNull.Value.Equals(current[6]) ? 0 : Convert.ToInt32(current[6]) });
                    }
                    break;
            }
        }
    }
}