using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngineAssignment_NO_MVC.Indexer
{
    public class WordStructure
    {
        private string document;
        private int id;
        private int termfrequency;

        public int TermFrequency
        {
            get { return termfrequency; }
            set { termfrequency = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Document
        {
            get { return document; }
            set { document = value; }
        }

        public WordStructure(string document, int id, int termfrequency)
        {
            this.Document = document;
            this.Id = id;
            this.TermFrequency = termfrequency;
        }
    }
}