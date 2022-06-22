using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using QVICommonIntranet.Database;
namespace REA_Tracker.Models
{
    public class NewsItem
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Url { get; set; }
        public int Index { get; set; }
    }

    public class NewsManager
    {
        public List<NewsItem> GenerateNews()
        {
            List<NewsItem> newsItems = new List<NewsItem>();

            HttpContext context = HttpContext.Current;

            String cmdText = "SELECT top 5 ST_TRACK.TRACKING_ID, ST_TRACK.TITLE FROM ST_TRACK WHERE ST_TRACK.ASSIGNED_TO = " + 
                Convert.ToString(context.Session["st_userID"]) +
                " order by ASSIGNED_ON desc";

            REATrackerDB sql = new REATrackerDB();

            DataTable dt = sql.ProcessCommand(cmdText);

            var request = HttpContext.Current.Request;

            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                NewsItem newsItem = new NewsItem()
                {
                    Author = Convert.ToString(context.Session["CurrentUserName"]),
                    Body = Convert.ToString(row[1]),
                    Title = "REA # - " + Convert.ToString(row[0]) + ": " + Convert.ToString(row[1]),
                    PublishedDate = DateTime.Now,
                    ImageUrl = "/Content/largetile.png",
                    Url = request.Url.GetLeftPart(UriPartial.Authority).ToString()+"/REA/Display/"+Convert.ToString(row[0]),
                    Index = i
                };
                newsItems.Add(newsItem);
                i++;
            }

            return newsItems;
        }
    }

    public class FeedResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }

        private readonly SyndicationFeedFormatter feed;
        public SyndicationFeedFormatter Feed
        {
            get { return feed; }
        }

        public FeedResult(SyndicationFeedFormatter feed)
        {
            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/rss+xml";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (feed != null)
                using (var xmlWriter = new XmlTextWriter(response.Output))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    feed.WriteTo(xmlWriter);
                }
        }
    }
}