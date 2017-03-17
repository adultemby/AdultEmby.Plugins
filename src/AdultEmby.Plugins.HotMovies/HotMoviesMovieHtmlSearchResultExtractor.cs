using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Core.HtmlExtractorUtils;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesMovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public HotMoviesMovieHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();

            IEnumerable<IElement> contentSearchResultElements =
                htmlDocument.QuerySelectorAll("div.movie_box");
            foreach (var searchResultElement in contentSearchResultElements)
            {
                try
                {
                    var item = new SearchResult();

                    IHtmlAnchorElement titleAnchor =
                        (IHtmlAnchorElement)
                        searchResultElement.QuerySelector("h3.title a");
                    if (titleAnchor != null)
                    {
                        item.Name = titleAnchor.Title;
                        string id = Part(titleAnchor.Href, '/', 4);
                        if (!string.IsNullOrEmpty(id))
                        {
                            item.Id = id;
                        }
                        item.Url = titleAnchor.Href;
                    }

                    IHtmlImageElement imageElement =
                        (IHtmlImageElement) searchResultElement.QuerySelector("td.movie_cover img.boxcover");
                    if (imageElement != null && imageElement.HasAttribute("style"))
                    {
                        string pattern = @".*url\((.*)\).*";

                        Match match = Regex.Match(imageElement.Attributes["style"].Value, pattern);
                        if (match.Success)
                        {
                            item.ImageUrl = match.Groups[1].Value;
                        }
                    }

                    var dateElement = searchResultElement.QuerySelector("span.release_year a");

                    if (dateElement != null)
                    {
                        string yearText = Text(dateElement);
                        int year;
                        if (int.TryParse(yearText, out year))
                        {
                            item.Year = year;
                        }
                    }

                    /*List<string> overview = new List<string>();
                    IElement siteElement = searchResultElement.Children.FirstOrDefault(m =>
                        m.LocalName == "p" &&
                        m.Text().Contains("Site:"));
                    if (siteElement != null)
                    {
                        overview.Add(siteElement.Text());
                    }
                    IElement networkElement = searchResultElement.Children.FirstOrDefault(m =>
    m.LocalName == "p" &&
    m.Text().Contains("Network:"));
                    if (networkElement != null)
                    {
                        overview.Add(networkElement.Text());
                    }
                    IElement castElement = searchResultElement.Children.FirstOrDefault(m =>
    m.LocalName == "p" &&
    m.Text().Contains("Cast: "));
                    if (castElement != null)
                    {
                        overview.Add(castElement.Text());
                    }

                    item.Overview = String.Join(", ", overview.ToArray());*/
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
