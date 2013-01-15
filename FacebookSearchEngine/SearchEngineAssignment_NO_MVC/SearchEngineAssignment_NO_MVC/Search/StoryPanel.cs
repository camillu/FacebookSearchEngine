using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SearchEngineAssignment_NO_MVC.Search
{
    public class StoryPanel : Panel
    {
        public StoryPanel(string cssClass)
        {
            CssClass = cssClass;
        }

        public void AddDiv(StoryPanel toAdd)
        {
            Controls.Add(toAdd);
        }

        public StoryPanel AddDiv(string cssClass)
        {
            StoryPanel panel = new StoryPanel(cssClass);
            Controls.Add(panel);
            return panel;
        }

        public Literal AddLiteral(string text)
        {
            Literal literal = new Literal();
            literal.Text = text;
            Controls.Add(literal);
            return literal;
        }

        public StoryPanel AddDivWithText(string cssClass, string text)
        {
            StoryPanel panel = new StoryPanel(cssClass);
            panel.AddLiteral(text);
            return panel;
        }

        public Label AddLabel(string cssClass, string text)
        {
            Label label = new Label();
            label.Text = text;
            label.CssClass = cssClass;
            Controls.Add(label);
            return label;
        }
        
        public void AddParagraph(string text)
        {
            AddLiteral(TextToHtml.ToParagraph(text));
        }

        public HyperLink AddLink(string cssClass, string text, string href)
        {
            HyperLink link = new HyperLink();
            link.Text = text;
            link.NavigateUrl = href;
            link.CssClass = cssClass;
            link.ToolTip = text;
            link.Target = "_parent";
            Controls.Add(link);
            return link;
        }

        public Image AddImage(string cssClass, string href)
        {
            Image image = new Image();
            image.ImageUrl = href;
            image.CssClass = cssClass;
            Controls.Add(image);
            return image;
        }
    }
}