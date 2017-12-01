using System.Collections.Generic;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionMovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public DorcelVisionMovieHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();

            IEnumerable<IElement> contentSearchResultElements =
                htmlDocument.QuerySelectorAll("a[class='movies rollover']");
            foreach (var searchResultElement in contentSearchResultElements)
            {
                //try
               // {
               IHtmlAnchorElement anchor = (IHtmlAnchorElement) searchResultElement;
                    var item = new SearchResult();

                

                string id = Part(anchor.Href, '/', 5) + '/' + Part(Part(anchor.Href, '/', 6), '.', 0);
                if (!string.IsNullOrEmpty(id))
                {
                    item.Id = id;
                }
                item.Url = anchor.Href;
                    
                IHtmlImageElement imageElement =
                        (IHtmlImageElement) searchResultElement.QuerySelector("img");
                if (imageElement != null)
                {
                    item.Name = imageElement.AlternativeText;
                    item.ImageUrl = imageElement.Source;
                }
                item.Year = 0;
                if (!string.IsNullOrEmpty(item.Name))
                {
                    results.Add(item);
                }
            }
            return results;
        }
    }
}
