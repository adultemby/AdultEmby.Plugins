using System;
using System.Globalization;
using System.Linq;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdPersonHtmlExtractor : IHtmlPersonExtractor
    {
        private ILogger _logger;

        public IafdPersonHtmlExtractor(ILogManager logManager)
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
            return Text(htmlDocument.QuerySelector("div.col-xs-12 h1"));
        }

        public DateTime? GetBirthdate(IHtmlDocument htmlDocument)
        {
            DateTime? birthdate = null;
            IElement birthdateElement = GetSummaryElement(htmlDocument, "Birthday");
            if (birthdateElement != null)
            {
                birthdate = ToDateTime(Text(birthdateElement.QuerySelector("a")), "MMMM dd, yyyy");
            }
            
            return birthdate;
        }

        public string GetStarSign(IHtmlDocument htmlDocument)
        {
            return GetSummaryData(htmlDocument, "Astrology");
        }

        public string GetMeasurements(IHtmlDocument htmlDocument)
        {
            return GetVitalStatsData(htmlDocument, "Measurements");
        }

        public string GetHeight(IHtmlDocument htmlDocument)
        {
            return GetVitalStatsData(htmlDocument, "Height");
        }

        public string GetWeight(IHtmlDocument htmlDocument)
        {
            return GetVitalStatsData(htmlDocument, "Weight");
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
            return GetVitalStatsData(htmlDocument, "Nationality");
        }

        public string GetBirthplace(IHtmlDocument htmlDocument)
        {
            return GetSummaryData(htmlDocument, "Birthplace");
        }

        public string GetEthnicity(IHtmlDocument htmlDocument)
        {
            return GetVitalStatsData(htmlDocument, "Ethnicity");
        }

        public string GetPrimaryImageUrl(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement headshotElement = (IHtmlImageElement) htmlDocument.QuerySelector("div#headshot img");

            string url = null;
            if (headshotElement != null)
            {
                url = headshotElement.Source;
            }
            return url;
        }

        private string GetVitalStatsData(IHtmlDocument htmlDocument, string name)
        {
            string value = null;
            IHtmlDivElement infobox = (IHtmlDivElement) htmlDocument.QuerySelector("div#vitalbox");
            if (infobox != null)
            {
                IHtmlCollection<IElement> strongElements = infobox.QuerySelectorAll("p.bioheading");
                foreach (var strongElement in strongElements)
                {
                    string strongText = Text(strongElement);
                    if (!string.IsNullOrEmpty(strongText) && strongText == name)
                    {
                        var potentialTextElement = strongElement.NextSibling;
                        if (potentialTextElement is IHtmlParagraphElement)
                        {
                            value = Trim(potentialTextElement.Text());
                        }
                    }
                }
            }
            return value;
        }

        private string GetSummaryData(IHtmlDocument htmlDocument, string name)
        {
            string value = null;
            IElement element = GetSummaryElement(htmlDocument, name);
            if (element != null && element is IHtmlParagraphElement)
            {
                value = Trim(element.Text());
            }
            return value;
        }

        private IElement GetSummaryElement(IHtmlDocument htmlDocument, string name)
        {
            IElement element = null;
            IHtmlDivElement infobox = (IHtmlDivElement)htmlDocument.QuerySelector("div[class='col-xs-12 col-sm-3']");
            if (infobox != null)
            {
                IHtmlCollection<IElement> strongElements = infobox.QuerySelectorAll("p.bioheading");
                foreach (var strongElement in strongElements)
                {
                    string strongText = Text(strongElement);
                    if (!string.IsNullOrEmpty(strongText) && strongText == name)
                    {
                        element = strongElement.NextElementSibling;
                        break;
                    }
                }
            }
            return element;
        }
    }
}
