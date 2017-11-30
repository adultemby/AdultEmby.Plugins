using System.Collections.Generic;
//using System.Data;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Model.Logging;
using static AdultEmby.Plugins.Base.HtmlExtractorUtils;

namespace AdultEmby.Plugins.JacquieEtMichel
{
    public class JacquieEtMichelMovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public JacquieEtMichelMovieHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            var results = new List<SearchResult>();

            IEnumerable<IElement> contentSearchResultElements =
                htmlDocument.QuerySelectorAll("div[class='col-lg-3 col-md-4 col-sm-6 col-xs-6 video-item'] a");
            foreach (var searchResultElement in contentSearchResultElements)
            {
                //try
               // {
               IHtmlAnchorElement anchor = (IHtmlAnchorElement)searchResultElement;
                    var item = new SearchResult();

                item.Name = Text(searchResultElement.QuerySelector("p.title-video"));

                string id = Part(anchor.Href, '/', 5) + '/' + Part(Part(anchor.Href, '/', 6), '.', 0);
                if (!string.IsNullOrEmpty(id))
                {
                    item.Id = id;
                }
                item.Url = anchor.Href;
                    
                IHtmlImageElement imageElement =
                        (IHtmlImageElement) searchResultElement.QuerySelector("img.videoThumb");
                if (imageElement != null)
                {

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
