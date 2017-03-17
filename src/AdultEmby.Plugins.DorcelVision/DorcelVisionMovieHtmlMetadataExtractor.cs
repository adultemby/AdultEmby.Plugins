using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionMovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public DorcelVisionMovieHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.col-xs-12 h1"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            //string synopsis = null;
            IHtmlParagraphElement paragraphElement =
                (IHtmlParagraphElement) htmlDocument.QuerySelector("div#synopsis p");
            //if (paragraphElement != null && paragraphElement.ChildElementCount > 0)
            //{
            //    synopsis = Trim(paragraphElement.FirstChild.TextContent);
           // }
            return Text(paragraphElement);
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            IEnumerable<IElement> anchors =
                htmlDocument.QuerySelectorAll("a[href*='pratiques'] span span");
            List<string> genres = new List<string>();
            foreach (IElement element in anchors)
            {
                genres.Add(Text(element));
            }

            return genres;
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.infos a[href*='/films']"));
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            return null;
            //return Text(htmlDocument.QuerySelector("li span.publication"));
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            //return Text(htmlDocument.QuerySelector("li span.publication"));
            int? productionYear = null;
            IHtmlCollection<IElement> labels = htmlDocument.QuerySelectorAll("div.infos strong");

            IElement label = FindElementContainingText(labels, "Année de production :");
            if (label != null)
            {
                productionYear = ToInt(Trim(label.NextSibling.TextContent));
            }
            return productionYear;
            //return Part(Text(htmlDocument.QuerySelector("li span.publication")), '/', 2);
        }

        public MoviePerson GetDirector(IDocument htmlDocument)
        {
            return null;
        }

        public string GetUpcCode(IDocument htmlDocument)
        {
            return null;
        }

        public string GetPrimaryImageUrl(IDocument htmlDocument)
        {
            IHtmlImageElement imageElement = (IHtmlImageElement) (htmlDocument.QuerySelector(
                "a.cover img.responsive"));
            return Trim(imageElement.Source);
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            IEnumerable<IElement> actorWithProfileElements = htmlDocument.QuerySelectorAll("div.casting div.slides div");
            foreach (var actorWithProfileElement in actorWithProfileElements)
            {
                /*IHtmlAnchorElement anchorElement = (IHtmlAnchorElement)actorWithProfileElement;
                string personId = Part(anchorElement.Href, '/', 4);*/
                IHtmlAnchorElement anchor = (IHtmlAnchorElement)actorWithProfileElement.QuerySelector("a.oneline");
                if (anchor != null)
                {
                    MoviePerson person = new MoviePerson();
                    person.Id = Part(anchor.Href, '/', 4) + '/' + Part(anchor.Href, '/', 5);
                    person.Name = Text(actorWithProfileElement.QuerySelector("a.oneline strong"));
                    actors.Add(person);
                }

            }
            return actors;
        }

        public string GetSet(IDocument htmlDocument)
        {
            return null;
        }
    }
}
