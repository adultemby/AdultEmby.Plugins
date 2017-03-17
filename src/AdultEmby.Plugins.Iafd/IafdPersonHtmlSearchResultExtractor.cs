using System;
using System.Collections.Generic;
using System.Linq;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdPersonHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public IafdPersonHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();
            ProcessRows(htmlDocument.QuerySelectorAll("table#tblFem tbody tr"), results);
            ProcessRows(htmlDocument.QuerySelectorAll("table#tblMal tbody tr"), results);
            return results;
        }

        private void ProcessRows(IEnumerable<IElement> rows, List<SearchResult>  results)
        {
            

            foreach (var searchResultElement in rows)
            {
                try
                {
                    var imageElement = (IHtmlImageElement)searchResultElement.QuerySelector("td:nth-child(1) img");
                    var nameElement = (IHtmlAnchorElement)searchResultElement.QuerySelector("td:nth-child(2) a");
                    string name = Text(nameElement);
                    string id = Part(nameElement.Href, '/', 4) + "/" + Part(nameElement.Href, '/', 5);
                    var item = new SearchResult()
                    {
                        ImageUrl = imageElement.Source,
                        Id = id,
                        Name = name,
                        Url = nameElement.Href
                    };
                    results.Add(item);
                }
                catch (Exception e)
                {
                    _logger.Info(e.Message);
                }
            }
        }
    }
}
