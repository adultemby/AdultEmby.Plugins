using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Data18
{
    public class Data18MovieHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public Data18MovieHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div#centered.main2 div h1"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            string description = Text(htmlDocument.QuerySelector("div.gen12 p.gen12"));
            return Trim(StripPrefix("Description:", description));
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            IEnumerable<IElement> anchors =
                htmlDocument.QuerySelectorAll("div.gen12 p a[href*='/movies/']");
            List<string> genres = new List<string>();
            foreach (IElement element in anchors)
            {
                IHtmlAnchorElement anchor = (IHtmlAnchorElement) element;
                string genre = Text(anchor);
                string href = anchor.Href;
                if (genre != null && !HasSuffix(href, "#scenes"))
                {
                    genres.Add(genre);
                }
            }

            return genres;
        }

        public string GetStudio(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.gen12 p a[href*='/studios/']"));
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            IHtmlCollection<IElement> paras =
                htmlDocument.QuerySelectorAll("div[style='background: #F3F3F3; width: 640px; padding: 0px; margin-bottom: 5px;'] div.gen12 p");

            DateTime? releaseDate = null;
            foreach (var para in paras)
            {
                string text = Text(para);
                if (text != null && text.StartsWith("Production Year"))
                {
                    releaseDate = ToDateTime(Trim(StripPrefix("Release date:", Trim(Part(text, '|', 1)))), "MMMM, yyyy");
                    break;
                }
            }
            return releaseDate;
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            IHtmlCollection<IElement> paras =
                htmlDocument.QuerySelectorAll("div[style='background: #F3F3F3; width: 640px; padding: 0px; margin-bottom: 5px;'] div.gen12 p");

            int? productionYear = null;
            foreach (var para in paras)
            {
                string text = Text(para);
                if (text != null && text.StartsWith("Production Year"))
                {
                    productionYear = ToInt(Trim(StripPrefix("Production Year:", Trim(Part(text, '|', 0)))));
                    break;
                }
            }
            return productionYear;
        }

        public MoviePerson GetDirector(IDocument htmlDocument)
        {
            string directorName = Text(htmlDocument.QuerySelector("div.gen12 p a[href*='director=']"));

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
            string physicalItemIdAndUpcCode = Text(htmlDocument.QuerySelector(
                "div[style='float: left; width: 220px; overflow: hidden; padding: 5px;'] p.gen11"));
            string upcText = Trim(Part(physicalItemIdAndUpcCode, '-', 1));
            return Trim(StripPrefix("UPC:", upcText));
        }

        public string GetPrimaryImageUrl(IDocument htmlDocument)
        {
            IHtmlImageElement imageElement = (IHtmlImageElement) (htmlDocument.QuerySelector(
                "img[alt*='Cover']"));
            return Trim(imageElement.Source);
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            IEnumerable<IElement> actorWithProfileElements = htmlDocument.QuerySelectorAll("div.contenedor div");
            foreach (var actorWithProfileElement in actorWithProfileElements)
            {
                IHtmlAnchorElement anchorElement = (IHtmlAnchorElement)actorWithProfileElement.QuerySelector("a.gensmall");
                if (anchorElement != null)
                {
                    string personId = Part(anchorElement.Href, '/', 3);
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
            }

            IHtmlAnchorElement firstActorWithoutProfileAnchor = (IHtmlAnchorElement) htmlDocument.QuerySelector("div[style='padding: 10px; margin-top: 15px;'] p b + a");
            if (firstActorWithoutProfileAnchor != null)
            {
                IEnumerable<IElement> actorWithoutProfileElements = firstActorWithoutProfileAnchor.ParentElement.QuerySelectorAll("a");
                foreach (var actorWithoutProfileElement in actorWithoutProfileElements)
                {
                    IHtmlAnchorElement anchorElement =
                        (IHtmlAnchorElement) actorWithoutProfileElement;
                    if (anchorElement != null)
                    {
                        string personId = Part(anchorElement.Href, '/', 3);
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
