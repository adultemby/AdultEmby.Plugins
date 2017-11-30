using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdMovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public IafdMovieHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();

            IEnumerable<IElement> rows =
                htmlDocument.QuerySelectorAll("table#titleresult tbody tr");
            foreach (var searchResultElement in rows)
            {
                try
                {
                    var item = new SearchResult();
                    var titleAnchor = (IHtmlAnchorElement)searchResultElement.QuerySelector("td:nth-child(1) a");
                    var yearElement = (IHtmlTableDataCellElement)searchResultElement.QuerySelector("td:nth-child(2)");
                    var studioElement = (IHtmlTableDataCellElement)searchResultElement.QuerySelector("td:nth-child(2)");

                    if (titleAnchor != null)
                    {
                        item.Name = Text(titleAnchor);
                        string id = Part(titleAnchor.Href, '/', 4) + "/" + Part(titleAnchor.Href, '/', 5);
                        if (!string.IsNullOrEmpty(id))
                        {
                            item.Id = id;
                        }
                        item.Url = titleAnchor.Href;
                    }

                    /*IHtmlImageElement imageElement =
                        (IHtmlImageElement) searchResultElement.QuerySelector("td.movie_cover img.boxcover");
                    if (imageElement != null && imageElement.HasAttribute("style"))
                    {
                        string pattern = @".*url\((.*)\).*";

                        Match match = Regex.Match(imageElement.Attributes["style"].Value, pattern);
                        if (match.Success)
                        {
                            item.ImageUrl = match.Groups[1].Value;
                        }
                    }*/

                    int? year = ToInt(Text(yearElement));
                    if (year.HasValue)
                    {
                        item.Year = year;
                    }
                    results.Add(item);
                }
                catch (Exception)
                {
                }
            }
            return results;
        }
    }
}
