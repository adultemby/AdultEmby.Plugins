using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdMovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public IafdMovieHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.col-sm-12 h1"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div[itemProp='description']"));
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            return new List<string>();
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return GetSummaryData(htmlDocument, "Studio");
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            return null;
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            return ToInt(Text(htmlDocument.QuerySelector(("span[itemprop='copyrightYear']"))));
        }

        public MoviePerson GetDirector(IDocument htmlDocument)
        {
            IElement directorElement = GetSummaryElement(htmlDocument, "Director");

            MoviePerson director = null;
            if (directorElement != null)
            {
                IHtmlAnchorElement directorAnchorElement = (IHtmlAnchorElement) directorElement.QuerySelector("a");
                string name = Text(directorAnchorElement);
                if (name != null)
                {
                    director = new MoviePerson()
                    {
                        Name = name
                    };
                }
            }
            return director;
        }

        public string GetUpcCode(IDocument htmlDocument)
        {
            return null;
        }

        public string GetPrimaryImageUrl(IDocument htmlDocument)
        {
            return null;
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            IEnumerable<IElement> actorElements = htmlDocument.QuerySelectorAll("div.castbox p a");
            foreach (var actorWithProfileElement in actorElements)
            {
                IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) actorWithProfileElement;
                string personId = Part(anchorElement.Href, '/', 4) + "/" + Part(anchorElement.Href, '/', 5);
                if (!string.IsNullOrEmpty(personId))
                {
                    MoviePerson person = new MoviePerson
                    {
                        Id = personId,
                        Name = Text(anchorElement)
                    };

                    actors.Add(person);
                }
            }
            return actors;
        }

        public string GetSet(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("a[href*='/series/']"));
        }

        private string GetSummaryData(IDocument htmlDocument, string name)
        {
            string value = null;
            IElement element = GetSummaryElement(htmlDocument, name);
            if (element != null && element is IHtmlParagraphElement)
            {
                value = Trim(element.Text());
            }
            return value;
        }

        private IElement GetSummaryElement(IDocument htmlDocument, string name)
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
