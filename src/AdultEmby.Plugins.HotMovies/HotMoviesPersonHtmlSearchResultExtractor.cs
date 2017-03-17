using System;
using System.Collections.Generic;
using System.Linq;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesPersonHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public HotMoviesPersonHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            // Stars with profile pages
            // div[style=padding: 4px; width: 90px;]
            IEnumerable<IElement> profileSearchResultElements = htmlDocument.QuerySelectorAll("li.photo_parent");

            var result = new List<SearchResult>();

            foreach (var searchResultElement in profileSearchResultElements)
            {
                try
                {
                    var anchorElement = (IHtmlAnchorElement) searchResultElement.QuerySelector("a");
                    var imageElement = (IHtmlImageElement) searchResultElement.QuerySelector("img");
                    string name = Text(anchorElement);
                    string id = anchorElement.Href.Split('/')[4];
                    string imageUrl = null;
                    if (imageElement.HasAttribute("key"))
                    {
                        imageUrl = imageElement.Attributes["key"].Value;
                    }
                    var item = new SearchResult()
                    {
                        ImageUrl = imageUrl,
                        Id = id,
                        Name = name,
                        Url = anchorElement.Href
                    };
                    result.Add(item);
                }
                catch (Exception e)
                {
                    _logger.Info(e.Message);
                }
            }
            return result;
        }
    }
}
