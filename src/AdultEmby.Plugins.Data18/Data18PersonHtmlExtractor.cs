using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Data18
{
    public class Data18PersonHtmlExtractor : IHtmlPersonExtractor
    {
        private ILogger _logger;

        public Data18PersonHtmlExtractor(ILogManager logManager)
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
            Dictionary<string, string> birthdateCell = ParseBirthdateCell(htmlDocument);

            string birthdateString = null;
            DateTime? birthdate = null;
            if (birthdateCell.ContainsKey("birthdate"))
            {
                birthdateString = birthdateCell["birthdate"];
                
                if (birthdateString != null)
                {
                    DateTime date;

                    if (DateTime.TryParseExact(birthdateString, "dd MMMM yyyy", new CultureInfo("en-US"),
                        DateTimeStyles.None,
                        out date))
                    {
                        birthdate = date;
                    }
                }
            }
            return birthdate;
        }

        public string GetStarSign(IHtmlDocument htmlDocument)
        {
            Dictionary<string, string> birthdateCell = ParseBirthdateCell(htmlDocument);
            
            string starSign = null;
            if (birthdateCell.ContainsKey("starsign"))
            {
                starSign = birthdateCell["starsign"];
            }
            return starSign;
        }

        public string GetMeasurements(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Measurements:");
        }

        public string GetHeight(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Height:");
        }

        public string GetWeight(IHtmlDocument htmlDocument)
        {
            return GetInfoBoxData(htmlDocument, "Weight:");
        }

        public string GetTwitter(IHtmlDocument htmlDocument)
        {
            IHtmlAnchorElement twitterAnchor =
                (IHtmlAnchorElement)
                htmlDocument.QuerySelector("p.line1[style='margin-bottom: 3px; margin-left: 5px;'] a[href*='twitter']");
            string twitter = null;
            if (twitterAnchor != null)
            {
                twitter = twitterAnchor.Href;
            }
            return twitter;
        }

        //string GetOfficialSite(IHtmlDocument htmlDocument)
        //{
        //    return "";
       // }

        public string GetNationality(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement flagImageElement = (IHtmlImageElement)htmlDocument.QuerySelector("img[src*='/flags/']");
            string nationality = null;
            if (flagImageElement != null)
            {
                nationality = Trim(flagImageElement.AlternativeText);
            }
            return nationality;
        }

        public string GetBirthplace(IHtmlDocument htmlDocument)
        {
            IHtmlImageElement flagImageElement = (IHtmlImageElement) htmlDocument.QuerySelector("img[src*='/flags/']");
            string birthplace = "";
            if (flagImageElement != null)
            {
                birthplace = Text(flagImageElement.ParentElement.QuerySelector("span.gensmall"));
            }
            return birthplace;
        }

        public string GetEthnicity(IHtmlDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("a.italo"));
        }

        public string GetPrimaryImageUrl(IHtmlDocument htmlDocument)
        {
            return ImageSource((IHtmlImageElement) htmlDocument.QuerySelector("img[src*='/stars/']"));
        }

        private IHtmlTableDataCellElement GetInfoBox(IHtmlDocument htmlDocument)
        {
            return (IHtmlTableDataCellElement)htmlDocument.QuerySelector("td[style='padding: 4px;'][valign='top']");
        }

        private Dictionary<string, string> ParseBirthdateCell(IHtmlDocument htmlDocument)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            IHtmlTableDataCellElement infobox = GetInfoBox(htmlDocument);
            if (infobox != null)
            {
                IHtmlCollection<IElement> divs = infobox.QuerySelectorAll("div");

                IElement birthdateDiv = FindElementContainingText(divs, "Birthdate:");

                ExtractStarSign(birthdateDiv, result);
                ExtractBirthdate(birthdateDiv, result);
            }
            return result;
        }

        private void ExtractStarSign(IElement birthdateDiv, Dictionary<string, string> result)
        {
            string starsign = Text(birthdateDiv.QuerySelector("i"));
            if (!string.IsNullOrEmpty(starsign))
            {
                result.Add("starsign", starsign);
            }
        }

        private void ExtractBirthdate(IElement birthdateDiv, Dictionary<string, string> result)
        {
            string birthdateString = "";
            IHtmlAnchorElement birthdateElement = (IHtmlAnchorElement) birthdateDiv.QuerySelector("a[href*='month=']");

            if (birthdateElement != null)
            {
                string monthAndDate = birthdateElement.Text();
                monthAndDate = String.Join(" ", monthAndDate.Split(new char[] { ' ', '\n' },
                    StringSplitOptions.RemoveEmptyEntries));
                string year = birthdateElement.NextSibling.Text();
                year = year.Replace(",", "");
                birthdateString = (monthAndDate + year).Trim();
                if (!string.IsNullOrEmpty(birthdateString))
                {
                    result.Add("birthdate", birthdateString);
                }
            }
        }

        private string GetInfoBoxData(IHtmlDocument htmlDocument, string name)
        {
            string value = null;
            IHtmlTableDataCellElement infobox = GetInfoBox(htmlDocument);
            if (infobox != null)
            {
                IHtmlCollection<IElement> divElements = infobox.QuerySelectorAll("div.gensmall");
                foreach (var divElement in divElements)
                {
                    IHtmlCollection<IElement> paragraphElements = divElement.QuerySelectorAll("p.line2");
                    if (paragraphElements.Length == 2)
                    {
                        string nameText = Text(paragraphElements.ElementAt(0));
                        string valueText = Text(paragraphElements.ElementAt(1));

                        if (!string.IsNullOrEmpty(nameText) && nameText.Contains(name))
                        {
                            value = valueText;
                            break;
                        }
                    }
                }
            }
            return value;
        }
    }
}
