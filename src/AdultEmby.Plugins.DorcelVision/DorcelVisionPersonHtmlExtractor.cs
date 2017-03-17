using System;
using System.Linq;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionPersonHtmlExtractor : IHtmlPersonExtractor
    {
        private ILogger _logger;

        public DorcelVisionPersonHtmlExtractor(ILogManager logManager)
        {
            _logger = logManager.GetLogger(GetType().FullName);
        }

        public bool HasMetadata(IHtmlDocument htmlDocument)
        {
            bool hasMetadata = true;
            IElement metaElement = htmlDocument.All.FirstOrDefault(m => m.LocalName == "meta" &&
                                      m.HasAttribute("name") && m.Attributes["name"].Value == "HAS_METADATA");
            if (metaElement != null && metaElement.HasAttribute("content") && metaElement.Attributes["content"].Value == "FALSE")
            {
                hasMetadata = false;
            }
            return hasMetadata;
        }

        public string GetName(IHtmlDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.container h1"));
        }

        public DateTime? GetBirthdate(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetStarSign(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetMeasurements(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetHeight(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetWeight(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetTwitter(IHtmlDocument htmlDocument)
        {
            return null;
        }

        //string GetOfficialSite(IHtmlDocument htmlDocument)
        //{
        //    return "";
       // }

        public string GetNationality(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement flagImageElement = (IHtmlImageElement) htmlDocument.QuerySelector("div.bubbles div img[src*='/flag']");
            string nationality = null;
            if (flagImageElement != null)
            {
                nationality = Text(flagImageElement.ParentElement.ParentElement);
            }
            return nationality;
        }

        public string GetBirthplace(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetEthnicity(IHtmlDocument htmlDocument)
        {
            return null;
        }

        public string GetPrimaryImageUrl(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement imageElement = (IHtmlImageElement) htmlDocument.QuerySelector("div.inner-slider a img");

            string url = null;
            if (imageElement != null)
            {
                url = imageElement.Source;
            }
            return url;
        }
    }
}
