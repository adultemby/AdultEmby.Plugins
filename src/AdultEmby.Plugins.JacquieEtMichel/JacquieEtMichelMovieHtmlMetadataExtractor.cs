using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.JacquieEtMichel
{
    public class JacquieEtMichelMovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public JacquieEtMichelMovieHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.video-player h1"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            IHtmlParagraphElement paragraphElement =
                (IHtmlParagraphElement) htmlDocument.QuerySelector("div.video-description p");
            return Trim(paragraphElement.FirstChild.TextContent);
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            IEnumerable<IElement> anchors =
                htmlDocument.QuerySelectorAll("span.categories a");
            List<string> genres = new List<string>();
            foreach (IElement element in anchors)
            {
                string genre = Text(element);
                if (!string.IsNullOrEmpty(genre))
                {
                    genres.Add(Part(genre, ',', 0));
                }
            }

            return genres;
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return "Jacquie Et Michel";
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            return ToDateTime(Text(htmlDocument.QuerySelector("li span.publication")), "dd/MM/yyyy");
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            return ToInt(Part(Text(htmlDocument.QuerySelector("li span.publication")), '/', 2));
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
                "div.video-player img"));
            return Trim(imageElement.Source);
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            return actors;
        }

        public string GetSet(IDocument htmlDocument)
        {
            return null;
        }
    }
}
