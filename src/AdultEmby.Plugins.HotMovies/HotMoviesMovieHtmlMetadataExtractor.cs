using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesMovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public HotMoviesMovieHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("h1 span[itemprop='name']"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div[itemProp='description']"));
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            IEnumerable<IElement> spans =
                htmlDocument.QuerySelectorAll("span[itemprop='genre']");
            List<string> genres = new List<string>();
            foreach (IElement element in spans)
            {
                string genre = Text(element);
                if (!string.IsNullOrEmpty(genre))
                {
                    genres.Add(genre);
                }
            }

            return genres;
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("span[itemprop='productionCompany'] span"));
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
            string directorName = Text(htmlDocument.QuerySelector("span[itemProp='director'] span"));

            MoviePerson director = null;
            if (directorName != null)
            {
                director = new MoviePerson()
                {
                    Name = directorName
                };
            }
            return director;
        }

        public string GetUpcCode(IDocument htmlDocument)
        {
            return null;
        }

        public string GetPrimaryImageUrl(IDocument htmlDocument)
        {
            IHtmlImageElement imageElement = (IHtmlImageElement) (htmlDocument.QuerySelector(
                "img#cover"));
            return Trim(imageElement.Source);
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            IEnumerable<IElement> actorWithProfileElements = htmlDocument.QuerySelectorAll("div.single_star div.name a");
            foreach (var actorWithProfileElement in actorWithProfileElements)
            {
                IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) actorWithProfileElement;
                string personId = Part(anchorElement.Href, '/', 4);
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
    }
}
