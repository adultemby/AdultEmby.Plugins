using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.Data18
{
    public class Data18MovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public Data18MovieHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();

            GetMovieSearchResults(htmlDocument, results);

            GetContentSearchResults(htmlDocument, results);
            return results;
        }

        private static void GetContentSearchResults(IDocument htmlDocument, List<SearchResult> result)
        {
            IEnumerable<IElement> contentSearchResultElements =
                htmlDocument.QuerySelectorAll("div.bscene");
            foreach (var searchResultElement in contentSearchResultElements)
            {
                try
                {
                    var item = new SearchResult();

                    IHtmlAnchorElement titleAnchor =
                        (IHtmlAnchorElement)
                        searchResultElement.QuerySelector("p[style='text-align: center;'] a[href*='/content/']");
                    if (titleAnchor != null)
                    {
                        item.Name = titleAnchor.Text();
                        string id = Part(titleAnchor.Href, '/', 4);
                        if (id != null)
                        {
                            item.Id = "content/" + id;
                            item.Url = titleAnchor.Href;
                        }
                    }

                    IHtmlImageElement imageElement =
                        (IHtmlImageElement) searchResultElement.QuerySelector("div[style='margin-bottom: 5px;'] img");
                    if (imageElement != null)
                    {
                        item.ImageUrl = imageElement.Source;
                    }

                    var dateElement = searchResultElement.QuerySelector("p.genmed");

                    if (dateElement != null)
                    {
                        DateTime date;
                        string dateText = dateElement.ChildNodes[1].Text();
                        if (DateTime.TryParseExact(Trim(dateText), "MMM dd, yyyy",
                            new CultureInfo("en-US"), DateTimeStyles.None, out date))
                        {
                            item.PremiereDate = date.ToUniversalTime();
                        }
                    }
                    result.Add(item);
                }
                catch (Exception)
                {
                }
            }
        }

        private void GetMovieSearchResults(IDocument htmlDocument, List<SearchResult> result)
        {
            IEnumerable<IElement> movieSearchResultElements =
                htmlDocument.QuerySelectorAll("div[style='float: left; padding: 6px; width: 130px;']");
            foreach (var searchResultElement in movieSearchResultElements)
            {
                try
                {
                    var a = searchResultElement.Children.ToList()[1];
                    string date = searchResultElement.ChildNodes[0].Text().Trim();
                    DateTime? dateTime = ToDateTime(date, "yyyy-MM-dd");
                    int year = 0;
                    if (dateTime.HasValue)
                    {
                        year = dateTime.Value.Year;
                    }
                    
                    IHtmlImageElement imageElement = null;
                    IHtmlAnchorElement imageAnchorElement = null;
                    IHtmlAnchorElement nameAnchorElement = null;
                    IHtmlCollection<IElement> anchors = searchResultElement.QuerySelectorAll("a");
                    foreach (var anchor in anchors)
                    {
                        IHtmlElement possibleImageElement = (IHtmlImageElement) anchor.QuerySelector("img");
                        if (possibleImageElement != null)
                        {
                            imageElement = (IHtmlImageElement) possibleImageElement;
                            imageAnchorElement = (IHtmlAnchorElement) anchor;
                        }
                        else
                        {
                            nameAnchorElement = (IHtmlAnchorElement) anchor;
                        }
                    }
                    string imageUrl = null;
                    string name = null;
                    string id = null;
                    string url = null;
                    if (imageElement != null)
                    {
                        imageUrl = imageElement.Source;
                        name = imageElement.AlternativeText;
                        id = Part(imageAnchorElement.Href, '/', 4);
                    }
                    if (nameAnchorElement != null)
                    {
                        name = nameAnchorElement.Text;
                        id = Part(nameAnchorElement.Href, '/', 4);
                        url = nameAnchorElement.Href;
                    }

                    string fullId = null;
                    if (id != null)
                    {
                        fullId = "movies/" + id;
                    }

                    var item = new SearchResult()
                    {
                        ImageUrl = imageUrl,
                        Relevance = 0.0,
                        Id = fullId,
                        Name = name,
                        PremiereDate = dateTime,
                        Year = year,
                        Url = url
                    };
                    result.Add(item);
                }
                catch (Exception)
                {
                    Console.WriteLine("");
                }
            }
        }
    }
}
