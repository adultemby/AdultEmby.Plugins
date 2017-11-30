using System;
using System.Collections.Generic;
using System.Resources;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Data18
{
    public class Data18ContentHtmlMetadataExtractor : IHtmlMetadataExtractor
    {
        private ILogger _logger;

        public Data18ContentHtmlMetadataExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }

        public string GetTitle(IDocument htmlDocument)
        {
            return Text(htmlDocument.QuerySelector("div.p8 div h1"));
        }

        public string GetSynopsis(IDocument htmlDocument)
        {
            string story =
                Text(
                    htmlDocument.QuerySelector(
                        "div.gen12[style='margin-top: 3px; margin-bottom: 15px; height: 60px; overflow-y: scroll;'] p"));
            return Trim(StripPrefix("Story:", story));
        }

        public List<string> GetGenres(IDocument htmlDocument)
        {
            IEnumerable<IElement> anchors =
                htmlDocument.QuerySelectorAll("div[style='padding: 3px;'] div[style='margin-top: 3px;'] a");
            List<string> genres = new List<string>();
            foreach (IElement anchor in anchors)
            {
                string genre = Text(anchor);
                if (genre != null)
                {
                    genres.Add(genre);
                }
            }

            return genres;
        }

        public string GetStudio(IDocument htmlDocument)
        {
            // TODO Echeck this is the studio element
            string studio =
                Text(
                    htmlDocument.QuerySelector(
                        "div[style='background: #F7F7F7; width: 624px; min-height: 218px; padding: 8px; margin-bottom: 3px; margin-top: 0px;'] p b+a"));
            return studio;
        }

        public DateTime? GetReleaseDate(IDocument htmlDocument)
        {
            return ToDateTime(Text(htmlDocument.QuerySelector("a[href*='content/date-']")), "MMMM d, yyyy");
        }

        public int? GetProductionYear(IDocument htmlDocument)
        {
            return null;
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
            IHtmlImageElement imageElement = (IHtmlImageElement) htmlDocument.QuerySelector("img[src*='media.data18.com/scenes']");
            return Trim(imageElement.Source);
        }

        public List<MoviePerson> GetActors(IDocument htmlDocument)
        {
            List<MoviePerson> actors = new List<MoviePerson>();
            if (HasWhosWhoData(htmlDocument))
            {
                IEnumerable<IElement> actorWithProfileElements =
                    htmlDocument.QuerySelectorAll("li[style='display: block; float: left; width: 135px;']");
                foreach (var actorWithProfileElement in actorWithProfileElements)
                {
                    IHtmlAnchorElement anchorElement =
                        (IHtmlAnchorElement) actorWithProfileElement.QuerySelector("a.bold");
                    if (anchorElement != null)
                    {
                        MoviePerson person = new MoviePerson();
                        person.Name = Text(anchorElement);
                        string personId = Part(anchorElement.Href, '/', 3);
                        if (personId != null)
                        {
                            person.Id = personId;
                        }
                        actors.Add(person);
                    }
                }

                IEnumerable<IElement> actorWithoutProfileElements =
                    htmlDocument.QuerySelectorAll("p.gen12[style='margin-top: 3px;'] a");
                foreach (IElement actorElement in actorWithoutProfileElements)
                {
                    IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) actorElement;
                    MoviePerson person = new MoviePerson();
                    person.Name = Text(actorElement);
                    string personId = Part(anchorElement.Href, '/', 4);
                    if (personId != null)
                    {
                        person.Id = personId;
                    }
                    actors.Add(person);
                }
            }
            else
            {
                IElement potentialStarringElement = htmlDocument.QuerySelector("div[style='background: #F7F7F7; width: 624px; min-height: 218px; padding: 8px; margin-bottom: 3px; margin-top: 0px;'] p b+a");
                if (potentialStarringElement != null)
                {
                    IHtmlCollection<IElement> sceneElements = potentialStarringElement.ParentElement.ParentElement.QuerySelectorAll("p");
                    IHtmlParagraphElement starringElement = (IHtmlParagraphElement) FindElementContainingText(sceneElements, "Starring:");
                    if (starringElement != null)
                    {
                        IHtmlCollection<IElement> actorElements = starringElement.QuerySelectorAll("a");
                        foreach (IElement actorElement in actorElements)
                        {
                            IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) actorElement;
                            MoviePerson person = new MoviePerson();
                            person.Name = Text(actorElement);
                            string personId = Part(anchorElement.Href, '/', 4);
                            if (personId != null)
                            {
                                person.Id = personId;
                            }
                            actors.Add(person);
                        }
                    }
                }
            }
            return actors;
        }

        public string GetSet(IDocument htmlDocument)
        {
            return null;
        }

        private bool HasWhosWhoData(IDocument htmlDocument)
        {
            return htmlDocument.QuerySelector("div[style='margin-right: 10px; margin-top: 30px;'] p.gen b") != null;
        }
    }
}
