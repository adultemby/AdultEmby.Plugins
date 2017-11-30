using System;
using System.Collections.Generic;
using System.Linq;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Data18
{
    public class Data18PersonHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public Data18PersonHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var result = new List<SearchResult>();

            ExtractSearchResultsWithProfiles(htmlDocument, result);

            ExtractSearchResultsWithoutProfiles(htmlDocument, result);

            return result;
        }

        private void ExtractSearchResultsWithoutProfiles(IDocument htmlDocument, List<SearchResult> result)
        {
            IHtmlDivElement noProfileSearchResultElement =
                (IHtmlDivElement) htmlDocument.QuerySelector("div.gen12[style='padding: 8px;']");

            if (noProfileSearchResultElement != null)
            {
                foreach (var element in noProfileSearchResultElement.QuerySelectorAll("a"))
                {
                    IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) element;
                    string id = Part(anchorElement.Href, '/', 3);
                    string name = Text(anchorElement);
                    var item = new SearchResult()
                    {
                        Id = id,
                        Name = name
                    };
                    result.Add(item);
                }
            }
        }

        private List<SearchResult> ExtractSearchResultsWithProfiles(IDocument htmlDocument, List<SearchResult> result)
        {
            IEnumerable<IElement> searchResultElements =
                htmlDocument.QuerySelectorAll("div[style='padding: 4px; width: 90px;']");
            
            foreach (var searchResultElement in searchResultElements)
            {
                IHtmlImageElement imageElement = (IHtmlImageElement) searchResultElement.QuerySelector("img");
                string imageUrl = null;
                if (imageElement != null && !imageElement.Source.Contains("no_prev"))
                {
                    imageUrl = imageElement.Source;
                }

                var anchorElements = searchResultElement.QuerySelectorAll("a");
                string name = null;
                string id = null;
                string url = null;
                if (anchorElements.Length == 2)
                {
                    IHtmlAnchorElement anchorElement = (IHtmlAnchorElement) anchorElements[1];
                    name = anchorElement.Text();
                    id = Trim(Part(anchorElement.Href, '/', 3));
                    url = anchorElement.Href;
                }
                var item = new SearchResult()
                {
                    ImageUrl = imageUrl,
                    Id = id,
                    Name = name,
                    Url = url
                };
                result.Add(item);
            }
            return result;
        }
    }
}
