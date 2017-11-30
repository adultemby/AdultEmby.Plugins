using System.Collections.Generic;
using System.IO;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.HotMovies;
using AdultEmby.Plugins.TestLogging;

namespace AdultEmby.Plugins.HotMovies.Test
{
    public class HotMoviesPersonHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractResult()
        {
            HotMoviesPersonHtmlSearchResultExtractor htmlSearchResultExtractor =
                new HotMoviesPersonHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            Assert.Equal(9, results.Count);
            Assert.Equal("4887", results[0].Id);
            Assert.Equal("Bree Olson", results[0].Name);
            Assert.Equal("https://www.hotmovies.com/porn-star/4887/Bree-Olson.html", results[0].Url);
            Assert.Equal("https://img3.vod.com/image2/star/488/Bree_Olson-4887.2.jpg", results[0].ImageUrl);

        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\PersonSearchResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
