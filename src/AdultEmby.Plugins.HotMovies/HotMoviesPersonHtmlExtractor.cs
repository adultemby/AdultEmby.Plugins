using System;
using System.Linq;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesPersonHtmlExtractor : IHtmlPersonExtractor
    {
        private ILogger _logger;

        public HotMoviesPersonHtmlExtractor(ILogManager logManager)
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
            return Text(htmlDocument.QuerySelector("h1.h1big"));
        }

        public DateTime? GetBirthdate(IHtmlDocument htmlDocument)
        {
            //IHtmlSpanElement birthdateSpan = (IHtmlSpanElement)htmlDocument.QuerySelector("span[itemprop='birthdate']");
            DateTime? birthdate = null;
            /*if (birthdateSpan != null)
            {
                try
                {
                    string birthdateString = Part(Text(birthdateSpan), '/', 0);

                        DateTime date;
                        
                        if (DateTime.TryParseExact(birthdateString, "MMMM dd yyyy", new CultureInfo("en-US"), DateTimeStyles.None,
                            out date))
                        {
                            birthdate = date;
                        }
                    
                }
                catch (Exception)
                {

                }
            }*/
            return birthdate;
        }

        public string GetStarSign(IHtmlDocument htmlDocument)
        {
            IHtmlSpanElement birthdateSpan = (IHtmlSpanElement) htmlDocument.QuerySelector("span[itemprop='birthdate']");

            return Trim(Part(Text(birthdateSpan), '/', 1));
        }

        public string GetMeasurements(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Breast Size:");
        }

        public string GetHeight(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Height:");
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
            return null;
        }

        public string GetBirthplace(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Birth Place:");
        }

        public string GetEthnicity(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Ethnicity:");
        }

        public string GetPrimaryImageUrl(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement starGalleryElement = (IHtmlImageElement) htmlDocument.QuerySelector("div.star_gallery_left img");
            IHtmlImageElement starImageElement = (IHtmlImageElement)htmlDocument.QuerySelector("img.star_image");

            string url = null;
            if (starGalleryElement != null)
            {
                url = starGalleryElement.Source;
            } else if (starImageElement != null)
            {
                url = starImageElement.Source;
            }
            return url;
        }

        private string GetInfoBoxData(IHtmlDocument htmlDocument, string name)
        {
            string value = null;
            IHtmlDivElement infobox = (IHtmlDivElement) htmlDocument.QuerySelector("div.star_info");
            if (infobox != null)
            {
                IHtmlCollection<IElement> strongElements = infobox.QuerySelectorAll("strong");
                foreach (var strongElement in strongElements)
                {
                    string strongText = Text(strongElement);
                    if (!string.IsNullOrEmpty(strongText) && strongText == name)
                    {
                        var potentialTextElement = strongElement.NextSibling;
                        if (potentialTextElement.NodeType == NodeType.Text)
                        {
                            value = Trim(potentialTextElement.Text());
                        }
                    }
                }
            }
            return value;
        }
    }
}
